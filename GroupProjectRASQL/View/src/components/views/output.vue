<template>
	<div>
		<div class="row">
			<column>
				<div v-html="model.output"></div>
			</column>
		</div>
		<div class="row">
			<column>
				<button class="btn btn-block btn-primary" @click="model.step.Execute('')">STEP</button>
			</column>
			<column>
				<button class="btn btn-block btn-primary" @click="model.stepToEnd.Execute('')">END</button>
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
		return {
			model: this.viewModel			
		}
	},
	methods: {

	}
}
</script>

<style>

</style>