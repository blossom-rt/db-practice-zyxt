import request from '../utils/request'

export function getMaterialCostPage(pageIndex, pageSize) {
  return request.get(`/MaterialCost/page?pageIndex=${pageIndex}&pageSize=${pageSize}`)
}

export function getMaterialCostByBill(billId) {
  return request.get(`/MaterialCost/${billId}`)
}

export function addMaterialCost(data) {
  return request.post('/MaterialCost', data)
}

export function editMaterialCost(data) {
  return request.put('/MaterialCost', data)
}

export function delMaterialCost(billId, matCode) {
  return request.delete(`/MaterialCost/${billId}/${matCode}`)
}
