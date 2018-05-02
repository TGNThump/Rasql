/*
*	File: integrated.js
*	Description: imports and installs vue and neutronium dependencies and sets options for the instance.
*/
import Vue from 'vue'
import App from './App.vue'
import { install, vueInstanceOption } from './install'
import vueHelper from 'vueHelper'

install(Vue)
vueHelper.setOption(vueInstanceOption)
Vue.component('app', App)