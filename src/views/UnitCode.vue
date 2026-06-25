<template>
  <div>
    <div style="margin-bottom:10px">
      <el-button v-if="isAdmin()" type="primary" @click="openDialog()">新增</el-button>
    </div>
    <el-table :data="tableData" border>
      <el-table-column prop="单位代码" label="单位代码"></el-table-column>
      <el-table-column prop="单位名称" label="单位名称"></el-table-column>
      <el-table-column v-if="isAdmin()" label="操作" width="160">
        <template #default="scope">
          <el-button type="primary" size="small" @click="openDialog(scope.row)">编辑</el-button>
          <el-button type="danger" size="small" @click="handleDel(scope.row.单位代码)">删除</el-button>
        </template>
      </el-table-column>
    </el-table>
    <el-pagination
      v-model:current-page="pageIndex"
      v-model:page-size="pageSize"
      :total="total"
      @change="loadData"
    ></el-pagination>
    <el-dialog v-model="visible" :title="isEdit ? '编辑单位代码' : '新增单位代码'">
      <el-form :model="form" @keyup.enter="submit">
        <el-form-item label="单位代码">
          <el-input v-model="form.单位代码" :disabled="isEdit"></el-input>
        </el-form-item>
        <el-form-item label="单位名称">
          <el-input v-model="form.单位名称"></el-input>
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
import { getUnitPage, addUnit, delUnit, editUnit } from '../api/unit'
import { isAdmin } from '../utils/auth'
import { ElMessage, ElMessageBox } from 'element-plus'

const pageIndex = ref(1)
const pageSize = ref(10)
const total = ref(0)
const tableData = ref([])
const visible = ref(false)
const isEdit = ref(false)
const form = ref({ 单位代码: '', 单位名称: '' })

async function loadData() {
  const res = await getUnitPage(pageIndex.value, pageSize.value)
  tableData.value = res.data.data
  total.value = res.data.total
}

function openDialog(row) {
  if (row) {
    isEdit.value = true
    form.value = { 单位代码: row.单位代码, 单位名称: row.单位名称 }
  } else {
    isEdit.value = false
    form.value = { 单位代码: '', 单位名称: '' }
  }
  visible.value = true
}

async function submit() {
  if (isEdit.value) {
    await editUnit(form.value)
    ElMessage.success('修改成功')
  } else {
    await addUnit(form.value)
    ElMessage.success('新增成功')
  }
  visible.value = false
  loadData()
}

async function handleDel(code) {
  try {
    await ElMessageBox.confirm('确定要删除该单位代码吗？此操作不可恢复', '警告', { type: 'warning', confirmButtonText: '确定删除', cancelButtonText: '取消' })
  } catch { return }
  await delUnit(code)
  ElMessage.success('删除成功')
  loadData()
}

onMounted(loadData)
</script>
