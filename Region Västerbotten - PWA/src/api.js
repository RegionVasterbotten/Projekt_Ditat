import axios from 'axios'

export default () => {
  return axios.create({
    baseURL: 'https://xapi.hi5.se/api',
    withCredentials: false,
    headers: {
      'Accept': 'application/json',
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('token')}`
    }
  })
}
