import request from '../utils/request'
export function getMaterialPage(pageIndex, pageSize) {
  return request.get(`/Material/page?pageIndex=${pageIndex}&pageSize=${pageSize}`)
}
export function addMaterial(data) {
  return request.post('/Material', data)
}
export function editMaterial(data) {
  return request.put('/Material', data)
}
export function delMaterial(code) {
  return request.delete(`/Material/${code}`)
}
export function getMaterialById(code) {
  return request.get(`/Material/${code}`)
}