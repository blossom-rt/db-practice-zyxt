import axios from 'axios'
import { ElMessage } from 'element-plus'
import { getToken, removeToken, removeRole, removeUsername } from './auth'

const service = axios.create({
  baseURL: 'http://localhost:5122/api',
  timeout: 10000
})

// 请求拦截 携带token
service.interceptors.request.use(config => {
  const token = getToken()
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

// 响应拦截 统一处理后端ApiResult
service.interceptors.response.use(res => {
  const data = res.data
  if (data.code !== 200) {
    ElMessage.error(data.msg)
    return Promise.reject(data)
  }
  return data
}, err => {
  if (err.response?.status === 401) {
    // token 无效或过期，清除登录态并跳回登录页
    removeToken()
    removeRole()
    removeUsername()
    ElMessage.warning('登录已过期，请重新登录')
    window.location.href = '/login'
    return Promise.reject(err)
  }
  const serverMsg = err.response?.data?.msg
  ElMessage.error(serverMsg || '接口请求失败：' + err.message)
  return Promise.reject(err)
})

export default service
