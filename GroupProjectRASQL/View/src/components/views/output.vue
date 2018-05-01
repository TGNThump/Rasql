<template>
	<div>
		<div v-if="model.SQL != ''" class="row" style="margin-bottom: 0px;">
			<column>
				<div class="card d-flex flex-row" style="margin-bottom: 0px; border-bottom-radius: 0px;">
					<div class="card-body" style="padding: 15px; text-align: right; width: 75px; background-color: #f7f7f7;">
						SQL
					</div>
					<div class="col card-body" style="padding: 15px;">
						{{model.SQL}}
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
						{{model.RA}}
					</div>
				</div>
			</column>
		</div>

		<div class="row">
			<column>
				<div class="card"><div class="card-body"><pre>{{ops}}</pre></div></div>
			</column>
		</div>


		<!-- <div class="row">
			<column>
        		<div v-html="model.Output"></div>
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
						<button @click="model.Step.Execute('')" class="btn btn-block btn-primary">Step</button>
					</column>
					<column>
						<button @click="model.Complete.Execute('')" class="btn btn-block btn-primary">Complete</button>
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
		</div> -->

		<div class="row">
			<column>
				<svg>
					<g ref="graph"></g>
				</svg>
			</column>
		</div>
	</div>
</template>

<script>
const props={
	viewModel: Object,
	__window__: Object
};

import * as d3 from 'd3';
export default {
	name: 'app',
	props,
	data () {
		return {
			model: this.viewModel		
		}
	},
	mounted(){
		var test = JSON.parse(`{
  "data": {
    "type": "Projection",
    "properties": "animals.name"
  },
  "children": [
    {
      "data": {
        "type": "Selection",
        "properties": "animals.age=&quot;3&quot;"
      },
      "children": [
        {
          "data": {
            "type": "Relation",
            "properties": "name, age, colour, weight"
          },
          "children": []
        }
      ]
    }
  ]
}`);

		var svg = d3.select(this.$refs.graph),
			width = +svg.attr("width"),
			height = +svg.attr("height"),
			g = svg.append("g");

		var data = d3.hierarchy(test);

		var i = 0;

		var tree = d3.tree(data).size([height,width]);

		var root = test[0];

		update(root);

		function update(source){
			var nodes = data.descendants().reverse();
			var links = data.links(nodes);

			nodes.forEach(function(d){d.y = d.depth * 180});

			var node = svg.selectAll("g.node")
				.data(nodes, function(d){return d.id || (d.id = ++i); });

			var nodeEnter = tree.node().enter().append("g")
				.attr("class","node")
				.attr("transform", function(d){
					return "translate(" + d.y + "," + d.x + ")";
				});

			nodeEnter.append("circle")
				.attr("r", 10)
				.style("fill", "#fff");

			nodeEnter.append("text")
				.attr("x", function(d){
					return d.children || d._children ? -13 : 13;
				}).attr("dy", ".35em")
				.attr("text-anchor", function(d){
					return d.children || d._children ? "end" : "start";
				}).text(function(d){return d.data.type;})
				.style("fill-opacity", 1);

			var link = svg.selectAll("path.link")
				.data(links, function(d) {return d.target.id;});

			link.enter().insert("path", "g")
				.attr("class", "link")
				.attr("d",
					d3.linkHorizontal()
					.x( function(d){
						return d.y;
					}).y( function(d){
						return d.x;
					})
				);
		}

	},
	computed: {
		ops: function(){
			if (this.model.OpsJSON == null) return null;
			if (this.model.OpsJSON == "") return null;
			return JSON.parse(this.model.OpsJSON);
		}
	}
}
</script>

<style>
 .node circle {
   fill: #fff;
   stroke: steelblue;
   stroke-width: 3px;
 }

 .node text { font: 12px sans-serif; }

 .link {
   fill: none;
   stroke: #ccc;
   stroke-width: 2px;
 }
</style>