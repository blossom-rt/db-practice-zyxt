import request from '../utils/request'
export function getUnitPage(pageIndex, pageSize) {
  return request.get(`/UnitCode/page?pageIndex=${pageIndex}&pageSize=${pageSize}`)
}
export function addUnit(data) {
  return request.post('/UnitCode', data)
}
export function editUnit(data) {
  return request.put('/UnitCode', data)
}
export function delUnit(code) {
  return request.delete(`/UnitCode/${code}`)
}
export function getUnitById(code) {
  return request.get(`/UnitCode/${code}`)
}