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
	document.getElementById("tb12").innerHTML = "NOT";
}

function raOnClick(){
	document.getElementById("tb1").innerHTML = "π";
	document.getElementById("tb2").innerHTML = "σ";
	document.getElementById("tb3").innerHTML = "ρ";
	document.getElementById("tb4").innerHTML = "∩";
	document.getElementById("tb5").innerHTML = "∪";
	document.getElementById("tb6").innerHTML = "⨯";
	document.getElementById("tb7").innerHTML = "⨝";
	document.getElementById("tb8").innerHTML = "⟕";
	document.getElementById("tb9").innerHTML = "⟖";
	document.getElementById("tb10").innerHTML = "⟗";
	document.getElementById("tb11").innerHTML = "⋉";
	document.getElementById("tb12").innerHTML = "⋊";
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

function switchViewsOnClick(){
	window.location = "view3.html";
}
