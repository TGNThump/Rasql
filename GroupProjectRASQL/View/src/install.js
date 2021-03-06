/*
*	File: install.js
*	Description: Defines install: require for vue, style and component dependencies.
*/
function install(Vue) {
    require('./assets/sass/main.scss');
	require('bootstrap');
    require('./components/components');
}

function vueInstanceOption() {
    //Return vue global option here, such as vue-router, vue-i18n, mix-ins, .... 
    return {}
}

export {
    install,
    vueInstanceOption
} 