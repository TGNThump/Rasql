<template>
	<div>
		<div class="row" style="margin-bottom: 0px;">
			<column>
				<div class="card d-flex flex-row" style="margin-bottom: 0px; border-bottom-radius: 0px;">
					<div class="card-body" style="padding: 15px; text-align: right; width: 75px; background-color: #f7f7f7;">
						SQL
					</div>
					<div class="col card-body" style="padding: 15px;">
						SELECT * FROM test WHERE a=b AND b=c AND d=e;
					</div>
				</div>
			</column>
		</div>

		<div class="row" style="margin-bottom: 0px;">
			<column>
				<div class="card d-flex flex-row" style="border-top-radius: 0px; border-top-width: 0px;">
					<div class="card-body" style="padding: 15px; text-align: right; width: 75px; background-color: #f7f7f7;">
						RA
					</div>
					<div class="col card-body" style="padding: 15px;">
						RA: σ a=b AND b=c AND d=e (test)
					</div>
				</div>
			</column>
		</div>

		<div class="row">
			<column>
				<div class="card">
					<div class="card-body" style="height: 500px;" v-html="model.output">
            
					</div>
				</div>
			</column>
			<column>
				<div class="row d-flex flex-row">
					<column style="flex-grow: 0; padding-right: 0px;">
						<button style="height: 100%; border-top-right-radius: 0px; border-bottom-right-radius: 0px; background-color: #F7F7F7;" class="btn btn-default">&lt;</button>
					</column>
					<column style="padding: 0px;">
						<div class="card" style="margin-bottom: 0px; border-radius: 0px;">
							<div class="card-header">The Selection Split Heuristic</div>
							<div class="card-body">This deals with the splitting of any selection - σp(r) - statement that has more than one condition, for example  σa=b and b=c  ,into several smaller selections.</div>
						</div>
					</column>
					<column style="flex-grow: 0; padding-left: 0px;">
						<button style="height: 100%; border-top-left-radius: 0px; border-bottom-left-radius: 0px; background-color: #F7F7F7;" class="btn btn-default">&gt;</button>
					</column>
				</div>
				<div class="row">
					<column>
						<button @click="model.step.Execute('')" class="btn btn-block btn-primary">Step</button>
					</column>
					<column>
						<button @click="model.stepToEnd.Execute('')" class="btn btn-block btn-primary">Complete</button>
					</column>
					<column>
						<button class="btn btn-block btn-primary">Reset</button>
					</column>
				</div>
				<div class="row">
					<div class="input-group mb-1" style="margin-bottom: 0px;">
						<div class="input-group-prepend">
							<div class="input-group-text">
								<input type="checkbox" aria-label="Checkbox for following text input">
							</div>
						</div>
						<span class="form-control">Selection Split Heuristic</span>
					</div>
					<div class="input-group mb-1" style="margin-bottom: 0px;">
						<div class="input-group-prepend">
							<div class="input-group-text">
								<input type="checkbox" aria-label="Checkbox for following text input">
							</div>
						</div>
						<span class="form-control">Move Selection Heuristic</span>
					</div>
					<div class="input-group mb-1" style="margin-bottom: 0px;">
						<div class="input-group-prepend">
							<div class="input-group-text">
								<input type="checkbox" aria-label="Checkbox for following text input">
							</div>
						</div>
						<span class="form-control">Restriction Heuristic</span>
					</div>
					<div class="input-group mb-1" style="margin-bottom: 0px;">
						<div class="input-group-prepend">
							<div class="input-group-text">
								<input type="checkbox" aria-label="Checkbox for following text input">
							</div>
						</div>
						<span class="form-control">Equijoin Conversion Heuristic</span>
					</div>
					<div class="input-group mb-1" style="margin-bottom: 0px;">
						<div class="input-group-prepend">
							<div class="input-group-text">
								<input type="checkbox" aria-label="Checkbox for following text input">
							</div>
						</div>
						<span class="form-control">Move Projection Heuristic</span>
					</div>
				</div>
			</column>
		</div>

		<!-- <div class="row">
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
		</div> -->
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