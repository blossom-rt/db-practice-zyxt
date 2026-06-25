import request from '../utils/request'
export function getTeamPage(pageIndex, pageSize) {
  return request.get(`/ConstructUnit/page?pageIndex=${pageIndex}&pageSize=${pageSize}`)
}
export function addTeam(data) {
  return request.post('/ConstructUnit', data)
}
export function editTeam(oldName, data) {
  return request.put(`/ConstructUnit/${encodeURIComponent(oldName)}`, data)
}
export function delTeam(code) {
  return request.delete(`/ConstructUnit/${code}`)
}
export function getTeamById(code) {
  return request.get(`/ConstructUnit/${code}`)
}