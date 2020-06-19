import Axios, { AxiosResponse } from 'axios';
import { IActivity, IActivitiesEnvelope } from '../models/activity';
import { history } from '../..';
import { toast } from 'react-toastify';
import { IUser, IUserFormValues } from '../models/user';
import { IProfile, IPhoto } from '../models/profile';

Axios.defaults.baseURL = process.env.REACT_APP_API_URL;

Axios.interceptors.request.use(
  (config) => {
    const token = window.localStorage.getItem('jwt');
    if (token) config.headers.Authorization = `Bearer ${token}`;
    return config;
  },
  (error) => Promise.reject(error)
);

Axios.interceptors.response.use(undefined, (error) => {
  if (error.message === 'Network Error' && !error.response) {
    toast.error("Network error - can't connect to API!");
  }
  const { status, data, config, headers } = error.response;
  if (status === 404) {
    history.push('/notfound');
  }
  if (status === 401 && headers['www-authenticate']) {
    window.localStorage.removeItem('jwt');
    history.push('/');
    toast.info('Your session has expired, please login again');
  }
  if (
    status === 400 &&
    config.method === 'get' &&
    data.errors.hasOwnProperty('id')
  ) {
    history.push('/notfound');
  }
  if (status === 500) {
    toast.error('Server error!');
  }
  throw error.response;
});

const responseBody = (response: AxiosResponse) => response.data;

// Used for testing purposes
const sleep = (ms: number) => (response: AxiosResponse) =>
  new Promise<AxiosResponse>((resolve) =>
    setTimeout(() => resolve(response), ms)
  );

const requests = {
  get: (url: string) => Axios.get(url).then(responseBody),
  post: (url: string, body: {}) => Axios.post(url, body).then(responseBody),
  put: (url: string, body: {}) => Axios.put(url, body).then(responseBody),
  delete: (url: string) => Axios.delete(url).then(responseBody),
  postForm: (url: string, file: Blob) => {
    let formData = new FormData();
    formData.append('File', file);
    return Axios.post(url, formData, {
      headers: { 'Content-type': 'multipart/form-data' },
    }).then(responseBody);
  },
};

const Activities = {
  list: (params: URLSearchParams): Promise<IActivitiesEnvelope> =>
    Axios.get('/activities', { params: params }).then(responseBody),
  details: (id: string): Promise<IActivity> =>
    requests.get(`/activities/${id}`),
  create: (activity: IActivity) => requests.post('/activities', activity),
  update: (activity: IActivity) =>
    requests.put(`/activities/${activity.id}`, activity),
  delete: (id: string) => requests.delete(`/activities/${id}`),
  attend: (id: string) => requests.post(`/activities/${id}/attend`, {}),
  unattend: (id: string) => requests.delete(`/activities/${id}/attend`),
};

const User = {
  current: (): Promise<IUser> => requests.get('/user'),
  login: (user: IUserFormValues): Promise<IUser> =>
    requests.post('/user/login', user),
  register: (user: IUserFormValues): Promise<IUser> =>
    requests.post('/user/register', user),
};

const Profiles = {
  get: (username: string): Promise<IProfile> =>
    requests.get(`/profiles/${username}`),
  uploadPhoto: (photo: Blob): Promise<IPhoto> =>
    requests.postForm(`/photos`, photo),
  setMainPhoto: (id: string) => requests.post(`/photos/${id}/setMain`, {}),
  deletePhoto: (id: string) => requests.delete(`/photos/${id}`),
  update: (profile: Partial<IProfile>) => requests.put(`/profiles`, profile),
  follow: (userName: string) =>
    requests.post(`/profiles/${userName}/follow`, {}),
  unfollow: (userName: string) =>
    requests.delete(`/profiles/${userName}/follow`),
  listFollowings: (userName: string, predicate: string) =>
    requests.get(`/profiles/${userName}/follow?predicate=${predicate}`),
  listActivities: (userName: string, predicate: string) =>
    requests.get(`/profiles/${userName}/activities?predicate=${predicate}`),
};

export default {
  Activities,
  User,
  Profiles,
};
