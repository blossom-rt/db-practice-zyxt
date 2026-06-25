<template>
  <div>
    <div style="margin-bottom:10px">
      <el-button v-if="isAdmin()" type="primary" @click="openDialog()">新增油水井</el-button>
    </div>
    <el-table :data="tableData" border>
      <el-table-column prop="井号" label="井号"></el-table-column>
      <el-table-column prop="所属单位" label="所属单位代码"></el-table-column>
      <el-table-column prop="井别" label="井别"></el-table-column>
      <el-table-column v-if="isAdmin()" label="操作" width="160">
        <template #default="scope">
          <el-button type="primary" size="small" @click="openDialog(scope.row)">编辑</el-button>
          <el-button type="danger" size="small" @click="handleDel(scope.row.井号)">删除</el-button>
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
    <el-dialog v-model="visible" :title="isEdit ? '编辑油水井' : '新增油水井'">
      <el-form :model="form" @keyup.enter="submit">
        <el-form-item label="井号">
          <el-input v-model="form.井号" :disabled="isEdit"></el-input>
        </el-form-item>
        <el-form-item label="所属单位代码">
          <el-input v-model="form.所属单位"></el-input>
        </el-form-item>
        <el-form-item label="井别">
          <el-input v-model="form.井别"></el-input>
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
import { getWellPage, addWell, delWell, editWell } from '../api/OilWell'
import { isAdmin } from '../utils/auth'
import { ElMessage, ElMessageBox } from 'element-plus'

const pageIndex = ref(1)
const pageSize = ref(10)
const total = ref(0)
const tableData = ref([])
const visible = ref(false)
const isEdit = ref(false)
const form = ref({ 井号: '', 所属单位: '', 井别: '' })

async function loadData() {
  const res = await getWellPage(pageIndex.value, pageSize.value)
  tableData.value = res.data.data
  total.value = res.data.total
}

function openDialog(row) {
  if (row) {
    isEdit.value = true
    form.value = { 井号: row.井号, 所属单位: row.所属单位, 井别: row.井别 }
  } else {
    isEdit.value = false
    form.value = { 井号: '', 所属单位: '', 井别: '' }
  }
  visible.value = true
}

async function submit() {
  if (isEdit.value) {
    await editWell(form.value)
    ElMessage.success('修改成功')
  } else {
    await addWell(form.value)
    ElMessage.success('新增成功')
  }
  visible.value = false
  loadData()
}

async function handleDel(id) {
  try {
    await ElMessageBox.confirm('确定要删除该油水井吗？此操作不可恢复', '警告', { type: 'warning', confirmButtonText: '确定删除', cancelButtonText: '取消' })
  } catch { return }
  await delWell(id)
  ElMessage.success('删除成功')
  loadData()
}

onMounted(loadData)
</script>
