import Axios, { AxiosResponse } from 'axios';
import { IActivity } from '../models/activity';
import { history } from '../..';
import { toast } from 'react-toastify';

Axios.defaults.baseURL = 'http://localhost:5000/api/';

Axios.interceptors.response.use(undefined, (error) => {
  if (error.message === 'Network Error' && !error.response) {
    toast.error("Network error - can't connect to API!");
  }
  const { status, data, config } = error.response;
  if (status === 404) {
    history.push('/notfound');
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
  throw error;
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
};

const Activities = {
  list: (): Promise<IActivity[]> => requests.get('/activities'),
  details: (id: string): Promise<IActivity> =>
    requests.get(`/activities/${id}`),
  create: (activity: IActivity) => requests.post('/activities', activity),
  update: (activity: IActivity) =>
    requests.put(`/activities/${activity.id}`, activity),
  delete: (id: string) => requests.delete(`/activities/${id}`),
};

export default {
  Activities,
};
