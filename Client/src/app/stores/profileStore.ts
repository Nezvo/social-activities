import { RootStore } from './rootStore';
import { observable, action, runInAction, computed, reaction } from 'mobx';
import { IProfile, IPhoto, IUserActivity } from '../models/profile';
import agent from '../api/agent';
import { toast } from 'react-toastify';

export default class ProfileStore {
  rootStore: RootStore;
  constructor(rootStore: RootStore) {
    this.rootStore = rootStore;

    reaction(
      () => this.activeTab,
      (activeTab) => {
        if (activeTab === 3 || activeTab === 4) {
          const predicate = activeTab === 3 ? 'followers' : 'followings';
          this.loadFollowings(predicate);
        } else {
          this.followings = [];
        }
      }
    );
  }

  @observable profile: IProfile | null = null;
  @observable loadingProfile = true;
  @observable uploadingPhoto = false;
  @observable loading = false;
  @observable followings: IProfile[] = [];
  @observable activeTab: number = 0;
  @observable userActivities: IUserActivity[] = [];
  @observable loadingActivities = false;

  @computed get isCurrentUser() {
    if (this.rootStore.userStore.user && this.profile) {
      return this.rootStore.userStore.user.userName === this.profile.userName;
    } else {
      return false;
    }
  }

  @action loadUserActivities = async (userName: string, predicate?: string) => {
    this.loadingActivities = true;
    try {
      const activities = await agent.Profiles.listActivities(
        userName,
        predicate!
      );
      runInAction(() => {
        this.userActivities = activities;
      });
    } catch (error) {
      console.log(error);
      toast.error('Problem loading activities');
    } finally {
      runInAction(() => {
        this.loadingActivities = false;
      });
    }
  };

  @action setActiveTab = (activeIndex: number) => {
    this.activeTab = activeIndex;
  };

  @action loadProfile = async (username: string) => {
    this.loadingProfile = true;
    try {
      const profile = await agent.Profiles.get(username);
      runInAction(() => {
        this.profile = profile;
      });
    } catch (error) {
      console.log(error);
      toast.error('Problem loading profile');
    } finally {
      runInAction(() => {
        this.loadingProfile = false;
      });
    }
  };

  @action uploadPhoto = async (file: Blob) => {
    this.uploadingPhoto = true;
    try {
      const photo = await agent.Profiles.uploadPhoto(file);
      runInAction(() => {
        if (this.profile) {
          this.profile.photos.push(photo);
          if (photo.isMain && this.rootStore.userStore.user) {
            this.rootStore.userStore.user.image = photo.url;
            this.profile.image = photo.url;
          }
        }
      });
    } catch (error) {
      console.log(error);
      toast.error('Problem uploading photo');
    } finally {
      runInAction(() => {
        this.uploadingPhoto = false;
      });
    }
  };

  @action setMainPhoto = async (photo: IPhoto) => {
    this.loading = true;
    try {
      await agent.Profiles.setMainPhoto(photo.id);
      runInAction(() => {
        this.rootStore.userStore.user!.image = photo.url;
        this.profile!.photos.find((a) => a.isMain)!.isMain = false;
        this.profile!.photos.find((a) => a.id === photo.id)!.isMain = true;
        this.profile!.image = photo.url;
      });
    } catch (error) {
      console.log(error);
      toast.error('Problem setting photo as main');
    } finally {
      runInAction(() => {
        this.loading = false;
      });
    }
  };

  @action deletePhoto = async (photo: IPhoto) => {
    this.loading = true;
    try {
      await agent.Profiles.deletePhoto(photo.id);
      runInAction(() => {
        this.profile!.photos = this.profile!.photos.filter(
          (a) => a.id !== photo.id
        );
      });
    } catch (error) {
      console.log(error);
      toast.error('Problem deleting the photo');
    } finally {
      runInAction(() => {
        this.loading = false;
      });
    }
  };

  @action updateProfile = async (profile: Partial<IProfile>) => {
    this.loading = true;
    try {
      await agent.Profiles.update(profile);
      runInAction(() => {
        if (
          profile.displayName !== this.rootStore.userStore.user!.displayName
        ) {
          this.rootStore.userStore.user!.displayName = profile.displayName!;
        }
        this.profile = { ...this.profile!, ...profile };
      });
    } catch (error) {
      console.log(error);
      toast.error('Problem updating profile');
    } finally {
      runInAction(() => {
        this.loading = false;
      });
    }
  };

  @action follow = async (userName: string) => {
    this.loading = true;
    try {
      await agent.Profiles.follow(userName);
      runInAction(() => {
        this.profile!.following = true;
        this.profile!.followersCount++;
      });
    } catch (error) {
      console.log(error);
      toast.error('Problem following user');
    } finally {
      runInAction(() => {
        this.loading = false;
      });
    }
  };

  @action unfollow = async (userName: string) => {
    this.loading = true;
    try {
      await agent.Profiles.unfollow(userName);
      runInAction(() => {
        this.profile!.following = false;
        this.profile!.followersCount--;
      });
    } catch (error) {
      console.log(error);
      toast.error('Problem unfollowing user');
    } finally {
      runInAction(() => {
        this.loading = false;
      });
    }
  };

  @action loadFollowings = async (predicate: string) => {
    this.loading = true;
    try {
      const profiles = await agent.Profiles.listFollowings(
        this.profile!.userName,
        predicate
      );
      runInAction(() => {
        this.followings = profiles;
      });
    } catch (error) {
      console.log(error);
      toast.error('Problem loading followings');
    } finally {
      runInAction(() => {
        this.loading = false;
      });
    }
  };
}
