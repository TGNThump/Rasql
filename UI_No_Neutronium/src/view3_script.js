var editor;
$(document).ready(function(){
	var code = $(".TextArea")[0];
	editor =  CodeMirror.fromTextArea(code, {
		lineNumbers : true, 
	});  
});  
 
function parseOnClick(){
	var xml = editor.getValue(); 
	var x2js = new X2JS();
	alert("converting: "+xml);
	var jsonObj = x2js.xml_str2json( xml );

	editor.setValue(JSON.stringify(jsonObj));
	editor.focus();
	console.log(jsonObj);
	alert("coversion done!");
}