var editor;
$(document).ready(function(){
	var code = $(".TextArea")[0];
	editor =  CodeMirror.fromTextArea(code, {
		lineNumbers : true,
		mode: "sql"
	}); 
});  

// lets not talk about this - it works tho 
function sqlOnClick(){
	document.getElementById("tb1").innerHTML = "SELECT";
	document.getElementById("tb2").innerHTML = "FROM";
	document.getElementById("tb3").innerHTML = "WHERE";
	document.getElementById("tb4").innerHTML = "GROUP";
	document.getElementById("tb5").innerHTML = "HAVING";
	document.getElementById("tb6").innerHTML = "ORDER";
	document.getElementById("tb7").innerHTML = "ANY";
	document.getElementById("tb8").innerHTML = "BETWEEN";
	document.getElementById("tb9").innerHTML = "EXISTS";
	document.getElementById("tb10").innerHTML = "IN";
	document.getElementById("tb11").innerHTML = "LIKE";
	document.getElementById("tb12").innerHTML = "*";
}

function raOnClick(){
	document.getElementById("tb1").innerHTML = "\u03C0";
	document.getElementById("tb2").innerHTML = "\u03C3";
	document.getElementById("tb3").innerHTML = "\u03C1";
	document.getElementById("tb4").innerHTML = "\u22C2";
	document.getElementById("tb5").innerHTML = "\u22C3";
	document.getElementById("tb6").innerHTML = "\u2A2F";
	document.getElementById("tb7").innerHTML = "\u2A1D";
	document.getElementById("tb8").innerHTML = "\u22C9";
	document.getElementById("tb9").innerHTML = "\u22CA";
	document.getElementById("tb10").innerHTML = "-";
	document.getElementById("tb11").innerHTML = "\u2227";
	document.getElementById("tb12").innerHTML = "\u2228";
}
 		
function tButtonOnClick(id){
	var operator = document.getElementById(id).innerHTML;
	editor.setValue(editor.getValue() + operator);
	editor.focus();
}

function parseOnClick(){
	var code = editor.getValue();
	window.location = "view2.html";
}

function datasetCreationViewOnClick(){
	window.location = "view3.html";
}
