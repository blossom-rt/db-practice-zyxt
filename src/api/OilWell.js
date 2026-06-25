import request from '../utils/request'
export function getWellPage(pageIndex, pageSize) {
  return request.get(`/OilWell/page?pageIndex=${pageIndex}&pageSize=${pageSize}`)
}
export function addWell(data) {
  return request.post('/OilWell', data)
}
export function editWell(data) {
  return request.put('/OilWell', data)
}
export function delWell(id) {
  return request.delete(`/OilWell/${id}`)
}
export function getWellById(id) {
  return request.get(`/OilWell/${id}`)
}