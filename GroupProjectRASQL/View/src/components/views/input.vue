<template>
	<div class="container-fluid app d-flex flex-column">
		<div class="row d-flex flex-row justify-content-between" style="margin-bottom: 0px; flex-shrink: 0;">
			<ul class="nav nav-tabs">
				<li class="nav-item">
					<a @click="model.Input_Type = 'sql'" class="nav-link" :class="{active: model.Input_Type == 'sql'}" href="#">SQL</a>
				</li>
				<li class="nav-item">
					<a @click="model.Input_Type = 'ra'" class="nav-link" :class="{active: model.Input_Type == 'ra'}" href="#">RA</a>
				</li>
			</ul>
			<ul class="nav nav-tabs" v-if="model.Input_Type == 'ra'">
				<li class="nav-item"><a @click="insertChar" class="nav-link" href="#">&#x03C0</a></li>
				<li class="nav-item"><a @click="insertChar" class="nav-link" href="#">&#x03C3</a></li>
				<li class="nav-item"><a @click="insertChar" class="nav-link" href="#">&#x03C1</a></li>
				<li class="nav-item"><a @click="insertChar" class="nav-link" href="#">&#x2A1D</a></li>
				<li class="nav-item"><a @click="insertChar" class="nav-link" href="#">&#x22C3</a></li>
				<li class="nav-item"><a @click="insertChar" class="nav-link" href="#">&#x22C2</a></li>
				<li class="nav-item"><a @click="insertChar" class="nav-link" href="#">X</a></li>
				<li class="nav-item"><a @click="insertChar" class="nav-link" href="#">-</a></li>
			</ul>
		</div>
		<div class="row">
			<codemirror v-if="model.Input_Type == 'sql'" v-model="model.Input_SQL" class="form-control codemirror" :options="cmoptions"></codemirror>
			<codemirror ref="ra" v-if="model.Input_Type == 'ra'" v-model="model.Input_RA" class="form-control codemirror" :options="cmoptions"></codemirror>
		</div>
		<div class="row" style="flex-shrink: 0;">
			<button @click="parse" class="btn btn-block btn-primary">Parse</button>
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
			model: this.viewModel,
			cmoptions: {
				lineNumbers : true,
				mode: 'text/x-sql',
			},
		};
	},
	computed: {
    	codemirror() {
      		return this.$refs.ra.codemirror
    	}
    },
    methods: {
    	insertChar($event){
    		this.codemirror.replaceRange($event.target.innerHTML, this.codemirror.getCursor());
    		this.codemirror.focus();
    	},
    	parse(){
    		if (this.model.Input_Type == "sql"){
    			this.model.ParseSQL.Execute(this.model.Input_SQL);
    		} else {
    			this.model.ParseRA.Execute(this.model.Input_RA);
    		}
    	}
    }
}
</script>

<style>

</style>