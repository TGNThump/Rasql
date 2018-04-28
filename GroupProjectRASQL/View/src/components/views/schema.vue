<template>
	<div class="container-fluid app d-flex flex-column">
		<div class="row d-flex flex-row" style="flex-shrink: 0;">
			<column>
				<input class="form-control" type="text" placeholder="Relation"></input>
			</column>
			<column style="flex-grow: 0;">
				<button type="button" class="btn btn-primary">New Relation</button>
			</column>
		</div>
		<div class="row">
			<column>
				<div class='card' style='flex-shrink: 0;' v-for='relation in Relations'>
					<div class='card-header' style="padding: 5px;">	
						<div class="row d-flex flex-row" style="flex-shrink: 0; margin: 0px;">
							<column>
								<input class="form-control relationName" type="text" placeholder="Relation" v-model="relation.name"></input>
							</column>
							<column style="flex-grow: 0;">
								<button type="button" style="margin: 0px;" class="btn btn-default">Delete Relation</button>
							</column>
						</div>
					</div>
					<div class='card-body'>
						<table class="table table-hover">
							<thead>
								<tr>
									<th scope="col" v-for='field in relation.fields'>
										<input class="form-control fieldName" type="text" placeholder="Field" v-model="field.name"></input>
									</th>
									<th scope="col" class="newButton">
										<button @click="newColumn(relation)" type="button" class="btn btn-default">+</button>
									</th>
								</tr>
							</thead>
							<tbody>
								<tr v-for='(_,i) in relation.fields[0].values'>
									<td v-for='field in relation.fields'>
										<input class="form-control fieldValue" type="text" placeholder="Value" v-model="field.values[i]"></input>
									</td>
									<td class="newButton">
										<button @click="removeRow(relation, i)" type="button" class="btn btn-default">-</button>
									</td>
								</tr>
								<tr>
									<td v-for='(field,i) in relation.fields'>
										<button @click="removeColumn(relation, i)" :disabled="relation.fields.length < 2"  type="button" class="btn btn-block btn-default">-</button>
									</td>
									<td></td>
								</tr>
								<tr>
									<td style="background-color: transparent !important; "class="newButton" :colspan="relation.fields.length">
										<button @click="newRow(relation)" type="button" class="btn btn-block btn-default">+</button>
									</td>
									<td></td>
								</tr>
							</tbody>
						</table>
					</div>
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
	},
	methods: {

	}
}
</script>

<style>
.cardcontainer .card{
	margin-top: 10px;
}

.fieldName,
.relationName{
	font-weight: bold;
}

.relationName,
.fieldName,
.fieldValue{
	background-color: transparent; border: 0px;
}

.newButton{
	text-align: center;
	vertical-align: center;
}
</style>