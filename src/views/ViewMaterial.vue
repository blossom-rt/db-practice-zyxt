<template>
  <el-card>
    <div style="margin-bottom:10px">
      <el-button @click="loadData">刷新</el-button>
    </div>
    <el-table :data="tableData" border max-height="600">
      <el-table-column prop="单据号" label="单据号" fixed width="160"></el-table-column>
      <el-table-column prop="预算单位" label="预算单位" width="100"></el-table-column>
      <el-table-column prop="井号" label="井号" width="100"></el-table-column>
      <el-table-column prop="施工单位" label="施工单位" width="120"></el-table-column>
      <el-table-column prop="施工内容" label="施工内容" min-width="160"></el-table-column>
      <el-table-column prop="物码" label="物码" width="100"></el-table-column>
      <el-table-column prop="名称规格" label="名称规格" min-width="140"></el-table-column>
      <el-table-column prop="计量单位" label="单位" width="60"></el-table-column>
      <el-table-column prop="消耗数量" label="数量" width="70"></el-table-column>
      <el-table-column prop="单价" label="单价" width="90"></el-table-column>
    </el-table>
  </el-card>
</template>
<script setup>
import { ref, onMounted } from 'vue'
import { getViewMaterial } from '../api/project'

const tableData = ref([])

async function loadData() {
  const res = await getViewMaterial()
  tableData.value = res.data
}

onMounted(loadData)
</script>
