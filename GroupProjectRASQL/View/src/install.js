function install(Vue) {
    require('./assets/sass/main.scss');
	require('bootstrap');
}

function vueInstanceOption() {
    //Return vue global option here, such as vue-router, vue-i18n, mix-ins, .... 
    return {}
}

export {
    install,
    vueInstanceOption
} 