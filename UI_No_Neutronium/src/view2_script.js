/*************
* On Clicks  *  
*************/
var tree; // init tree - check view2_tree_display.js
$(document).ready(function(){
	tree_config = {
	    chart: {
	        container: "#tree-simple",
	        levelSeparation:    30,
	        siblingSeparation:  20,
	        subTeeSeparation:   50,
	        rootOrientation: "NORTH",

	        node: { 
	            HTMLclass: "node-draw",
	            drawLineThrough: false
	        },
	        connectors: {
	            type: "step",
	            style: {
	                "stroke-width": 2,
	                "stroke": "#ccc"
	            },
	            stackIndent: 20	
	        }
	    },
	    
	    nodeStructure: {
	        text: { name: "Node", desc: "subscript" },
	        children: [
	            {
	                text: { name: "Node", desc: "subscript" },
	                children: [
			            {
			                text: { name: "Node", desc: "subscript" },
			            },
			            {
	                		text: { name: "Node", desc: "subscript" }
	            		}
	            	],
	            },
	            {
	                text: { name: "Node", desc: "subscript" },
	                children: [
			            {
			                text: { name: "Node", desc: "subscript" },
			            },
			            {
	                		text: { name: "Node", desc: "subscript" }
	            		}
	            	],
	            }
	        ],
	    } 
	};

	tree = new Treant(tree_config, function() { alert( 'Tree Loaded' ) }, $ );
});  

function backToView1OnClick(){
	window.location = "index.html";
}

function helpOnClick(){

}

function applyHeuristicOnClick(){

}

function backOnClick(){
	
}

function fowardOnClick(){

}

function automateOnClick(){

}

function generateQueryOnClick(){

}