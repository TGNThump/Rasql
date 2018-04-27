<template>
	<div class="container-fluid app d-flex flex-column">
		Schema
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