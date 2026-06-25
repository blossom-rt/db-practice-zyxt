import request from '../utils/request'

// 分页查询（自动区分管理员/视图）
export function getProjectPage(params) {
  return request.get('/JobProject/page', { params })
}

// 组合查询
export function combinedQuery(params) {
  return request.get('/JobProject/query', { params })
}

// 新增项目+多条材料（事务接口）
export function addProjectWithMaterial(data) {
  return request.post('/JobProject/with-materials', data)
}

// 新增单个项目
export function createProject(data) {
  return request.post('/JobProject', data)
}

// 修改项目
export function updateProject(data) {
  return request.put('/JobProject', data)
}

// 删除项目（自动级联删明细）
export function delProject(billNo) {
  return request.delete(`/JobProject/${billNo}`)
}

// 获取单个项目
export function getProjectById(billId) {
  return request.get(`/JobProject/${billId}`)
}

// 存储过程统计成本
export function getCostStat() {
  return request.get('/JobProject/proc/cost')
}

// 三表联查视图（作业项目+材料费+物码）
export function getViewMaterial() {
  return request.get('/JobProject/view/material')
}
