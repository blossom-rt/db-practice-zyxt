<template>
  <div>
    <!-- 搜索栏 -->
    <el-card style="margin-bottom:16px">
      <el-form :model="filters" inline size="small" @keyup.enter="handleSearch">
        <el-form-item label="操作类型">
          <el-input v-model="filters.opType" placeholder="模糊搜索" clearable style="width:130px" />
        </el-form-item>
        <el-form-item label="操作表">
          <el-input v-model="filters.opTable" placeholder="模糊搜索" clearable style="width:130px" />
        </el-form-item>
        <el-form-item label="操作人">
          <el-input v-model="filters.opUser" placeholder="模糊搜索" clearable style="width:130px" />
        </el-form-item>
        <el-form-item label="操作时间">
          <el-date-picker
            v-model="dateRange"
            type="daterange"
            range-separator="至"
            start-placeholder="开始日期"
            end-placeholder="结束日期"
            value-format="YYYY-MM-DD"
            clearable
            style="width:250px"
          />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查询</el-button>
          <el-button @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 表格 -->
    <el-table :data="tableData" border style="width:100%">
      <el-table-column prop="id" label="Id" width="70"></el-table-column>
      <el-table-column prop="操作类型" label="操作类型" width="120"></el-table-column>
      <el-table-column prop="操作表" label="操作表" width="150"></el-table-column>
      <el-table-column prop="操作人" label="操作人" width="160"></el-table-column>
      <el-table-column prop="操作时间" label="操作时间" width="200"></el-table-column>
      <el-table-column prop="操作内容" label="操作内容" min-width="200" show-overflow-tooltip></el-table-column>
    </el-table>
    <el-pagination
      v-model:current-page="pageIndex"
      v-model:page-size="pageSize"
      :total="total"
      layout="total, prev, pager, next"
      @change="loadData"
      style="margin-top:12px"
    ></el-pagination>
  </div>
</template>
<script setup>
import { ref, onMounted, reactive } from 'vue'
import { getLogPage } from '../api/log'

const pageIndex = ref(1)
const pageSize = ref(10)
const total = ref(0)
const tableData = ref([])

const filters = reactive({
  opType: '',
  opTable: '',
  opUser: ''
})
const dateRange = ref(null)

function buildParams() {
  const params = { pageIndex: pageIndex.value, pageSize: pageSize.value }
  if (filters.opType) params.opType = filters.opType
  if (filters.opTable) params.opTable = filters.opTable
  if (filters.opUser) params.opUser = filters.opUser
  if (dateRange.value) {
    params.dateFrom = dateRange.value[0]
    params.dateTo = dateRange.value[1]
  }
  return params
}

async function loadData() {
  const res = await getLogPage(buildParams())
  tableData.value = res.data.data
  total.value = res.data.total
}

function handleSearch() {
  pageIndex.value = 1
  loadData()
}

function handleReset() {
  filters.opType = ''
  filters.opTable = ''
  filters.opUser = ''
  dateRange.value = null
  pageIndex.value = 1
  loadData()
}

onMounted(loadData)
</script>
