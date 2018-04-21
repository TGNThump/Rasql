var editor;
var current_jsonObj;

$(document).ready(function(){
	var code = $(".CodeArea")[0];
	editor =  CodeMirror.fromTextArea(code, {
		lineNumbers : true, 	 
	});   

	var json_str = localStorage.getItem('current_jsonObj');
	if(json_str != null){
		alert("Found an active data set! Previewing:");
		current_jsonObj = JSON.parse(json_str);
		editor.setValue(json_str);
		previewOnClick();
	}
});  

function saveOnClick(){
	if(editor.getValue().length < 1){
		alert("Editor is blank!")
		return;
	}

	var xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" 
	+ "<root>"  
	+ editor.getValue()
	+ "</root>";
	var x2js = new X2JS();
	alert("converting: "+xml);
	var jsonObj = x2js.xml_str2json( xml );

	var status = isValidJsonObject(jsonObj);
	
	alert(status[1]);
	if( !status[0] )
		return;
	
	editor.setValue(JSON.stringify(jsonObj));
	editor.focus();
	
	// Put the object into storage
	current_jsonObj = jsonObj;
	localStorage.setItem('current_jsonObj', JSON.stringify(jsonObj));
	console.log("storing json!");
}

function backToParserOnClick(){ 
	window.location = "index.html";
}

function previewOnClick(){
	if( current_jsonObj == undefined ){ 
		alert("Write valid XML first!!");
		return; 
	}

	var i = 0;
	$("#previewArea").empty();
	for (var key in current_jsonObj.root) {
		var id = "table" + i;
		var table = "<div class=\"TableTitle\">" + key + "</div>\n<table id = \"" + id + "\"> </table>";
		$("#previewArea").append(table);	
		buildHtmlTable(current_jsonObj.root[key], id);
		i++; 
	}
}

/******************** 
  JSON -> HTML TABLE
*********************/

function isValidJsonObject(jsonObj){
	if( jsonObj == null )
		return [false, "Document structure && syntax is incorrect!"];

	if( jsonObj.root != "[object Object]" )
		return [false, "Invalid initial document structure! (Probably just random stuff was entered)"];

	for (var key in jsonObj.root) { 
		var repeatCol = -1;
		var firstKey;
		for (var key2 in jsonObj.root[key]) {
			console.log(key2);
			if( !isNaN(key2) && repeatCol != 1 )
				return [false, "There is more than one table with the same name!"];
			if ( repeatCol == -1 ){ 
				repeatCol = jsonObj.root[key][key2].length;
			}else{
				if( repeatCol != jsonObj.root[key][key2].length )
					return [false, "There is a table with an unbalanced number of elements in a row. #elements_row = #columns (must)!"];;
			}
		}
	}

	return [ true, "Valid xml input! Nice!" ];
} 

function buildHtmlTable(table, id) {
	alert("Building table id: " + id);
	console.log(table);
	var columnSet = Object.keys(table);
	console.log( columnSet.length );  
	var headerTr$ = $('<tr/>');

	for (var i = 0 ; i < columnSet.length; i++) {
		headerTr$.append($('<th/>').html(columnSet[i]));
	} 
	$("#"+id).append(headerTr$);

	var colData = []; 
	var rows;
	var cols;

	var a = 0;
	for (var key in table) {
		rows = table[key].length;
		colData[a] = table[key];
		a++;
	}

	cols = a;

	console.log("rows: " + rows);
	console.log("cols: " + cols); 

	if(cols == 1){
		tr= $('<tr/>');
		tr.append("<td>" + colData[0] + "</td>");
		$("#"+id).append(tr);
	}else{
		for( var i = 0; i < rows; i++ ){
			tr= $('<tr/>');
			for( var j = 0; j < cols; j++ ){ 
				tr.append("<td>" + colData[j][i] + "</td>");
			}
			$("#"+id).append(tr);
		}
	}
} 	