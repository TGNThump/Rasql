<template>
	<div class="container-fluid app d-flex flex-column">
		<div class="row">
			<column>
				<input class="form-control" type="text" placeholder="" v-model="input"></input>
			</column>
			<div class="col-2">
				<button class="btn btn-block" @click="Parse.Execute('sql')">SQL</button>
			</div>
      <div class="col-2">
        <button class="btn btn-block" @click="Parse.Execute('ra')">RA</button>
      </div>
		</div>
		<div class="row" style="flex-grow: 1;">
			<column class="d-flex flex-column" style="flex-grow: 1;">
				<div style="overflow-y: scroll; overflow-x: hidden;" v-html="output">
					
				</div>
			</column>
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