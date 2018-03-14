<template>
  <div class="container-fluid app">
    <div class="row">
      <div class="col">
        <input class="form-control" type="text" placeholder="" v-model="input_sql"></input>
      </div>
      <div class="col-2">
        <button class="btn btn-block" @click="Parse.Execute()">Parse</button>
      </div>
    </div>
  </div>
</template>

<script>
const props={
  viewModel: Object,
  __window__: Object
};

export default {
  name: 'app',
  props,
  data () {
    return this.viewModel
  }
}
</script>

<style>

</style>