<template>
  <div>
    <div style="margin-bottom:10px;display:flex;gap:10px">
      <el-input v-model="searchBillId" placeholder="按单据号筛选" style="width:200px" clearable @clear="loadData" @keyup.enter="loadData"></el-input>
      <el-button @click="loadData">查询</el-button>
      <el-button v-if="isAdmin()" type="primary" @click="openDialog()">新增材料消耗</el-button>
    </div>
    <el-table :data="tableData" border>
      <el-table-column prop="单据号" label="单据号"></el-table-column>
      <el-table-column prop="物码" label="物码"></el-table-column>
      <el-table-column prop="消耗数量" label="消耗数量"></el-table-column>
      <el-table-column prop="单价" label="单价"></el-table-column>
      <el-table-column prop="金额" label="金额">
        <template #default="scope">
          {{ (scope.row.消耗数量 * scope.row.单价).toFixed(2) }}
        </template>
      </el-table-column>
      <el-table-column v-if="isAdmin()" label="操作" width="160">
        <template #default="scope">
          <el-button type="primary" size="small" @click="openDialog(scope.row)">编辑</el-button>
          <el-button type="danger" size="small" @click="handleDel(scope.row.单据号, scope.row.物码)">删除</el-button>
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

    <el-dialog v-model="visible" :title="isEdit ? '编辑材料消耗' : '新增材料消耗'">
      <el-form :model="form" label-width="80px" @keyup.enter="submit">
        <el-form-item label="单据号">
          <el-input v-model="form.单据号" :disabled="isEdit"></el-input>
        </el-form-item>
        <el-form-item label="物码">
          <el-input v-model="form.物码" :disabled="isEdit"></el-input>
        </el-form-item>
        <el-form-item label="消耗数量">
          <el-input v-model.number="form.消耗数量" type="number"></el-input>
        </el-form-item>
        <el-form-item label="单价">
          <el-input v-model.number="form.单价" type="number"></el-input>
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
import { getMaterialCostPage, getMaterialCostByBill, addMaterialCost, editMaterialCost, delMaterialCost } from '../api/materialCost'
import { isAdmin } from '../utils/auth'
import { ElMessage, ElMessageBox } from 'element-plus'

const pageIndex = ref(1)
const pageSize = ref(10)
const total = ref(0)
const tableData = ref([])
const searchBillId = ref('')
const visible = ref(false)
const isEdit = ref(false)
const form = ref({ 单据号: '', 物码: '', 消耗数量: 0, 单价: 0 })

async function loadData() {
  if (searchBillId.value) {
    const res = await getMaterialCostByBill(searchBillId.value)
    tableData.value = res.data
    total.value = res.data.length
  } else {
    const res = await getMaterialCostPage(pageIndex.value, pageSize.value)
    tableData.value = res.data.data
    total.value = res.data.total
  }
}

function openDialog(row) {
  if (row) {
    isEdit.value = true
    form.value = { 单据号: row.单据号, 物码: row.物码, 消耗数量: row.消耗数量, 单价: row.单价 }
  } else {
    isEdit.value = false
    form.value = { 单据号: '', 物码: '', 消耗数量: 0, 单价: 0 }
  }
  visible.value = true
}

async function submit() {
  if (!form.value.单据号 || !form.value.物码) {
    ElMessage.warning('单据号和物码不能为空')
    return
  }
  if (form.value.消耗数量 <= 0) {
    ElMessage.warning('消耗数量必须大于0')
    return
  }
  if (form.value.单价 < 0) {
    ElMessage.warning('单价不能为负数')
    return
  }
  if (isEdit.value) {
    await editMaterialCost(form.value)
    ElMessage.success('修改成功')
  } else {
    await addMaterialCost(form.value)
    ElMessage.success('新增成功')
  }
  visible.value = false
  loadData()
}

async function handleDel(billId, matCode) {
  try {
    await ElMessageBox.confirm('确定要删除该材料消耗明细吗？此操作不可恢复', '警告', { type: 'warning', confirmButtonText: '确定删除', cancelButtonText: '取消' })
  } catch { return }
  await delMaterialCost(billId, matCode)
  ElMessage.success('删除成功')
  loadData()
}

onMounted(loadData)
</script>
