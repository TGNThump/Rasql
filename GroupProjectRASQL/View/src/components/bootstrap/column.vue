<template>
	<div v-bind:class="classes">
		<slot></slot>
	</div>
</template>

<script type="text/javascript">
	export default{
		props: {
			xs: String,
			sm: String,
			md: String,
			lg: String
		},
		methods: {
			getSize: function getSize(prefix){
				var size = this[prefix];
				if (typeof size == 'undefined'){ return; }
				var ret = []
				size.split(" ").forEach(part => {
					if (part == "hidden") ret.push(part + "-" + prefix);
					else ret.push("col-" + prefix + "-" + part);
				});
				return ret;
			},
		},
		computed: {
			classes: function(){
				var ret = ["col"]
				ret.push(this.getSize("xs"));
				ret.push(this.getSize("sm"));
				ret.push(this.getSize("md"));
				ret.push(this.getSize("lg"));
				return ret;
			},
		},
	}
</script>
