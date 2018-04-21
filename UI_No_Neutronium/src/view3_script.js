var editor;
var current_jsonObj;

$(document).ready(function(){
	var code = $(".CodeArea")[0];
	editor =  CodeMirror.fromTextArea(code, {
		lineNumbers : true, 	
	});  
});  

function saveOnClick(){
	var xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" 
	+ "<root>"  
	+ editor.getValue()
	+ "</root>";
	var x2js = new X2JS();
	alert("converting: "+xml);
	var jsonObj = x2js.xml_str2json( xml );

	var status = isValidJsonObject(jsonObj);
	if( !status[0] ){
		// invalid
	}else{
		editor.setValue(JSON.stringify(jsonObj));
		editor.focus();
	}
	alert(status[1]);
	current_jsonObj = jsonObj;
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
	if( jsonObj.root != "[object Object]" )
		return [false, "Invalid initial document structure! (Probably just random stuff was entered)"];

	for (var key in jsonObj.root) { 
		var repeatCol = -1;
		var firstKey;
		for (var key2 in jsonObj.root[key]) {
			console.log(key2);
			if( !isNaN(key2) )
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
	alert("test");
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