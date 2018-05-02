<!--
	File: output.vue 
	Description: Output view markup for the user interface view 1.
				 Here the user iteracts with the parsed query's tree.
				 He is allowed to implement heuristics via UI
																   		--> 
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
			<column lg="6">
				<div class="card">
					<div class="card-body">
						<svg id="graph"></svg>
					</div>
				</div>
			</column>
			<column lg="6">
				<div v-if="model.CurrentHeuristic != null" class="row d-flex flex-row">
					<column style="flex-grow: 0; padding-right: 0px;">
						<!-- <button style="height: 100%; border-top-right-radius: 0px; border-bottom-right-radius: 0px; background-color: #F7F7F7;" class="btn btn-default">&lt;</button> -->
					</column>
					<column style="padding: 0px;">
						<div class="card" style="margin-bottom: 0px;">
						<!-- <div class="card" style="margin-bottom: 0px; border-radius: 0px;"> -->
							<div class="card-header">{{model.CurrentHeuristic.name}}</div>
							<div class="card-body">{{model.CurrentHeuristic.description}}</div>
						</div>
					</column>
					<column style="flex-grow: 0; padding-left: 0px;">
						<!-- <button style="height: 100%; border-top-left-radius: 0px; border-bottom-left-radius: 0px; background-color: #F7F7F7;" class="btn btn-default">&gt;</button> -->
					</column>
				</div>
				<div class="row">
					<column>
						<button @click="model.Step.Execute()" class="btn btn-block btn-primary">Step</button>
					</column>
					<column>
						<button @click="model.Complete.Execute()" class="btn btn-block btn-primary">Complete</button>
					</column>
					<column>
						<button @click="model.Reset.Execute()" class="btn btn-block btn-primary">Reset</button>
					</column>
				</div>
				<div class="row">
					<!-- <pre>{{model.HeuristicsArray}}</pre> -->
					<div class="input-group mb-1" style="margin-bottom: 0px;" v-for="heuristic in model.HeuristicsArray">
						<div class="input-group-prepend">
							<div class="input-group-text">
								<input type="checkbox" v-model="heuristic.isEnabled" aria-label="Checkbox for following text input">
							</div>
						</div>
						<span class="form-control">{{heuristic.name}}</span>
						<!-- <div class="card" style="flex-grow: 0;"><div class="card-body"><pre>{{heuristic}}</pre></div></div> -->
					</div>
					<!-- <div class="input-group mb-1" style="margin-bottom: 0px;">
						<div class="input-group-prepend">
							<div class="input-group-text">
								<input type="checkbox" aria-label="Checkbox for following text input">
							</div>
						</div>
						<span class="form-control">Selection Split Heuristic</span>
					</div> -->
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

  String.prototype.replaceAll = function(search, replacement) {
    var target = this;
    return target.split(search).join(replacement);
  };

  import * as d3 from 'd3';
  export default {
  name: 'app',
  props,
  data () {
  return {
  model: this.viewModel,
  svg: null,
  }
  },
  computed: {
  ops: function(){
  if (this.model.OpsJSON == null) return null;
  if (this.model.OpsJSON == "") return null;
  return JSON.parse(this.model.OpsJSON);
  }
  },
  watch: {
  ops: function (value){
  this.render();
  }
  },
  mounted(){
  this.svg = d3.select('#graph');
  this.svg.append("g").attr("transform", "translate(0,50)")
  this.render();
  },
  methods: {
  render: function(){
  if (this.ops == null) return;
  if (this.ops == "") return;
  var root = d3.hierarchy(this.ops);

  var svg = this.svg;
  var	width = (root.leaves().length+1) * 200,
  height = Math.min((root.leaves()[0].depth) * 200, 500);

  d3.cluster().size([width, height])(root);

  svg.attr("viewBox","0 0 " + (width + 100) + " " + (height + 100));

  var g = svg.select("g");

  // Update

  var link = g.selectAll('.link')
  .data(root.links())
  .attr('x1', function(d) {return d.source.x;})
  .attr('y1', function(d) {return d.source.y;})
  .attr('x2', function(d) {return d.target.x;})
  .attr('y2', function(d) {return d.target.y;});

  var node = g.selectAll(".node")
  .data(root.descendants())
  .attr("class", function (d){
  return "node" + (d.children ? " node--internal" : " node--leaf");
  }).attr("transform", function(d) {
  return "translate(" + d.x + "," + d.y + ")";
  });

  node.select('text')
  .attr("x", function(d){ return d.children ? -8 : 8; })
  .style("text-anchor", function(d){ return d.children ? "end" : "start";})
  .text(function(d){ return d.data.data.type + ' '+ d.data.data.properties.replaceAll("&quot;", "\""); });

			// Enter

			var link = g.selectAll(".link")
				.data(root.links())
				.enter()
				.append('line')
				.classed('link', true)
				.attr('x1', function(d) {return d.source.x;})
				.attr('y1', function(d) {return d.source.y;})
				.attr('x2', function(d) {return d.target.x;})
				.attr('y2', function(d) {return d.target.y;});

			var node = g.selectAll(".node")
				.data(root.descendants())
				.enter().append("g")
					.attr("class", function (d){
						return "node" + (d.children ? " node--internal" : " node--leaf");
					}).attr("transform", function(d) {
						return "translate(" + d.x + "," + d.y + ")";
					});
		
			node.append("circle").attr("r", 2.5);

			node.append("text")
				.attr("dy", -3)
				.attr("x", function(d){ return d.children ? -8 : 8; })
				.style("text-anchor", function(d){ return d.children ? "end" : "start";})
				.text(function(d){ return d.data.data.type + ' '+ d.data.data.properties.replaceAll("&quot;", "\""); });

			// Exit

			g.selectAll(".link").data(root.links()).exit().remove();
			g.selectAll(".node").data(root.descendants()).exit().remove();
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