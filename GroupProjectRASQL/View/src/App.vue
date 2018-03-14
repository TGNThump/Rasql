<template>
  <div class="container-fluid app">
    <div class="card">
      <div class="card-body">{{viewModel}}</div>
    </div>
    <br />
    <button class="btn" @click="TestCommand.Execute()">printToConsole</button>
    <button class="btn" @click="test++">increment</button>
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