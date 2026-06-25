<template>
  <div>
    <div style="margin-bottom:10px">
      <el-button v-if="isAdmin()" type="primary" @click="openDialog()">新增施工单位</el-button>
    </div>
    <el-table :data="tableData" border>
      <el-table-column prop="施工单位名称" label="施工单位名称"></el-table-column>
      <el-table-column v-if="isAdmin()" label="操作" width="160">
        <template #default="scope">
          <el-button type="primary" size="small" @click="openDialog(scope.row)">编辑</el-button>
          <el-button type="danger" size="small" @click="handleDel(scope.row.施工单位名称)">删除</el-button>
        </template>
      </el-table-column>
    </el-table>
    <el-pagination
      v-model:current-page="pageIndex"
      v-model:page-size="pageSize"
      :total="total"
      @change="loadData"
      style="margin-top:10px"
    ></el-pagination>
    <el-dialog v-model="visible" :title="isEdit ? '编辑施工单位' : '新增施工单位'">
      <el-form :model="form" @keyup.enter="submit">
        <el-form-item label="施工单位名称">
          <el-input v-model="form.施工单位名称"></el-input>
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="visible=false">取消</el-button>
        <el-button type="primary" @click="submit">确定</el-button>
      </template>
    </el-dialog>
  </div>
</template>
<script setup>
import { ref, onMounted } from 'vue'
import { getTeamPage, addTeam, delTeam, editTeam } from '../api/ConstructUnit'
import { isAdmin } from '../utils/auth'
import { ElMessage, ElMessageBox } from 'element-plus'

const pageIndex = ref(1)
const pageSize = ref(10)
const total = ref(0)
const tableData = ref([])
const visible = ref(false)
const isEdit = ref(false)
const oldName = ref('')
const form = ref({ 施工单位名称: '' })

async function loadData() {
  const res = await getTeamPage(pageIndex.value, pageSize.value)
  tableData.value = res.data.data
  total.value = res.data.total
}

function openDialog(row) {
  if (row) {
    isEdit.value = true
    oldName.value = row.施工单位名称
    form.value = { 施工单位名称: row.施工单位名称 }
  } else {
    isEdit.value = false
    oldName.value = ''
    form.value = { 施工单位名称: '' }
  }
  visible.value = true
}

async function submit() {
  if (isEdit.value) {
    await editTeam(oldName.value, form.value)
    ElMessage.success('修改成功')
  } else {
    await addTeam(form.value)
    ElMessage.success('新增成功')
  }
  visible.value = false
  loadData()
}

async function handleDel(name) {
  try {
    await ElMessageBox.confirm('确定要删除该施工单位吗？此操作不可恢复', '警告', { type: 'warning', confirmButtonText: '确定删除', cancelButtonText: '取消' })
  } catch { return }
  await delTeam(name)
  ElMessage.success('删除成功')
  loadData()
}

onMounted(loadData)
</script>
