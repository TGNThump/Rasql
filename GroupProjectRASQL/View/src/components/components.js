require('./components.vendor');

import input from './views/input.vue'
import output from './views/output.vue'
import schema from './views/schema.vue'

Vue.component('view-input', input);
Vue.component('view-output', output);
Vue.component('view-schema', schema);

import column from './bootstrap/column.vue'

Vue.component('column', column);