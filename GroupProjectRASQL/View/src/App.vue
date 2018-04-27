<template>
  <div class="container-fluid app d-flex flex-column">
    <nav class="nav-main navbar navbar-dark bg-primary">
      <a class="navbar-brand" href="#">Relational Algebra Educational Tool</a>

      <ul class="navbar-nav" style="flex-direction: row;">
        <li class="nav-item" :class="{active: CurrentView == 'input'}">
          <a class="nav-link" @click="CurrentView = 'input'" href="#">Input</a>
        </li>
        <li class="nav-item" :class="{active: CurrentView == 'output'}">
          <a class="nav-link" @click="CurrentView = 'output'" href="#">Output</a>
        </li>
        <li class="nav-item" :class="{active: CurrentView == 'schema'}">
          <a class="nav-link" @click="CurrentView = 'schema'" href="#">Schema</a>
        </li>
      </ul>
    </nav>

    <div class="app" style="overflow-y: scroll; overflow-x: hidden;">
      <keep-alive>
    		<view-input class="view" v-if="CurrentView == 'input'" :viewModel='this.viewModel'></view-input>
      </keep-alive>
      <keep-alive>
        <view-output class="view" v-if="CurrentView == 'output'" :viewModel='this.viewModel'></view-output>
      </keep-alive>
      <keep-alive>
        <view-schema class="view" v-if="CurrentView == 'schema'" :viewModel='this.viewModel'></view-schema>
      </keep-alive>
    </div>
    
    <nav class="navbar navbar-light bg-light">
      <span class="navbar-text text-muted text-small">
        <span class="badge">Group 17:</span><small>Anna, Ben, Duncan, Paulo, Tom and Robin.</small>
      </span>
    </nav>
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
	},
  methods: {
    
  }
}
</script>

<style>

</style>