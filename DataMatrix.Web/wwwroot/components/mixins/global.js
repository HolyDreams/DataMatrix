import vue from 'vue/dist/vue.esm.js';

var allComponents = [];
var registerGlobalMixins = function() {
    vue.mixin({
        created: function() {
            allComponents.push(this);
        },
        mounted: function() {
            if (this === this.$root)
                document.body.style.display = 'block';
        }
    });
}

export { allComponents, registerGlobalMixins };