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
		</div>
		<div class="row" style="flex-grow: 1;">
			<column class="d-flex flex-column" style="flex-grow: 1;">
        <div style="overflow-y: scroll; overflow-x: hidden;" class="cardcontainer">
          <div class='card' style='flex-shrink: 0;'>
            <div class='card-header'>Schema</div>
            <div class='card-body'>
              <div v-for='relation in Relations'>
                <b>{{relation.name}}</b>
                <table class="table">
                  <thead>
                    <tr>
                      <th scope="col" v-for='field in relation.fields'>{{field.name}}</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for='(_,i) in relation.fields[0].values'>
                      <td v-for='field in relation.fields'>{{field.values[i]}}</td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>
          </div>
          <div v-html="output"></div>
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
  .cardcontainer .card{
    margin-top: 10px;
  }
</style>