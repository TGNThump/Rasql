<!--
	File: input.vue 
	Description: Input markup for the user interface view 1.
				 Here the user inputs the SQL/RA to be parsed.
															   -->
<template>
	<div class="container-fluid app d-flex flex-column">
		<div class="row" style="margin-bottom: 0px; flex-shrink: 0;">
			<column class="d-flex flex-row justify-content-between">
				<ul class="nav nav-tabs">
					<!-- Selection between RA/SQL input buttons --> 
					<li class="nav-item">
						<a @click="model.Input_Type = 'sql'" class="nav-link" :class="{active: model.Input_Type == 'sql'}" href="#">SQL</a>
					</li>
					<li class="nav-item">
						<a @click="model.Input_Type = 'ra'" class="nav-link" :class="{active: model.Input_Type == 'ra'}" href="#">RA</a>
					</li>
				</ul>
				<ul class="nav nav-tabs" v-if="model.Input_Type == 'ra'">
					<!-- Input characters buttons for RA -->
					<li class="nav-item"><a @click="insertChar" class="nav-link" href="#">&#x03C0</a></li>
					<li class="nav-item"><a @click="insertChar" class="nav-link" href="#">&#x03C3</a></li>
					<li class="nav-item"><a @click="insertChar" class="nav-link" href="#">&#x03C1</a></li>
					<li class="nav-item"><a @click="insertChar" class="nav-link" href="#">&#8904</a></li>
					<li class="nav-item"><a @click="insertChar" class="nav-link" href="#">U</a></li>
					<li class="nav-item"><a @click="insertChar" class="nav-link" href="#">N</a></li>
					<li class="nav-item"><a @click="insertChar" class="nav-link" href="#">X</a></li>
					<li class="nav-item"><a @click="insertChar" class="nav-link" href="#">-</a></li>
				</ul>
			</column>
		</div>
		<div class="row">
			<column>
				<!-- Text box Area: using code mirror -->
				<codemirror style="border-radius: 0px 0px inherit inherit;" :class="{'is-invalid': !model.Input_Valid_SQL}" v-if="model.Input_Type == 'sql'" v-model="model.Input_SQL" class="form-control codemirror" :options="cmoptions"></codemirror>
				<codemirror :class="{'is-invalid': !model.Input_Valid_RA}" ref="ra" v-if="model.Input_Type == 'ra'" v-model="model.Input_RA" class="form-control codemirror" :options="cmoptions"></codemirror>
			</column>
		</div>
		<div class="row" style="flex-shrink: 0;">
			<column>
				<button @click="parse" class="btn btn-block btn-primary">Parse</button>
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
			model: this.viewModel,
			cmoptions: {
				lineNumbers : true,
				mode: 'text/x-sql',
			},
		};
	},
	computed: {
		// codemirror integration (Multi-Lined Textbox)
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