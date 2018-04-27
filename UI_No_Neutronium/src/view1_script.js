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
	document.getElementById("taskbar sql").hidden = false;
	document.getElementById("taskbar ra").hidden = true;
}

function raOnClick(){
	document.getElementById("taskbar sql").hidden = true;
	document.getElementById("taskbar ra").hidden = false;
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
