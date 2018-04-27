<template>
	<div class="container-fluid app d-flex flex-column">
		<div class="row" style="flex-shrink: 0;">
			<column>
				<input class="form-control" type="text" placeholder="" v-model="input"></input>
			</column>
			<div class="col-2">
				<button class="btn btn-block" @click="Parse.Execute('sql')">SQL</button>
			</div>
      <div class="col-2">
        <button class="btn btn-block" @click="Parse.Execute('ra')">RA</button>
      </div>
      <div class="col-2">
        <button class="btn btn-block" @click="step.Execute('')">STEP</button>
      </div>
      <div class="col-2">
        <button class="btn btn-block" @click="stepToEnd.Execute('')">END</button>
      </div>

		</div>
		<div class="row" style="flex-grow: 1;">
			<column class="d-flex flex-column" style="flex-grow: 1;">
        <div style="overflow-y: scroll; overflow-x: hidden;" class="cardcontainer">
          <div v-html="output"></div>
          <div class='card' style='flex-shrink: 0;'>
            <div class='card-header'>Schema</div>
            <div class='card-body'>
              <div v-for='relation in Relations'>
                <input class="form-control relationName" type="text" placeholder="Relation" v-model="relation.name"></input>
                <!-- <pre>{{relation}}</pre> -->
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
    newRow: function(relation){
      for (var i = relation.fields.length - 1; i >= 0; i--) {
        relation.fields[i].values.push("");
      }
    },
    removeRow: function(relation, row){
       for (var i = relation.fields.length - 1; i >= 0; i--) {
        relation.fields[i].values.splice(row, 1);
      }
    },
    newColumn: function(relation){
      relation.fields.push({
        name: "",
        values: []
      });
    },
    removeColumn: function(relation, column){
      if (relation.fields.length == 1) return;
      relation.fields.splice(column, 1);
    }
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