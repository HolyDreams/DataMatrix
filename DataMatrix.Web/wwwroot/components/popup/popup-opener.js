export default {
    props: {
        forPopup: {
            type: String,
            required: true
        },
        openWhenMouseEnter: {
            type: Boolean,
            default: false
        }
    },
    mounted: function() {
        var component = this;
        Array.from(component.$el.children).forEach(function(child) {
            child.addEventListener(component.openWhenMouseEnter === true ? 'mouseenter' : 'click',
                function() {
                    const popup = component.$parent.$refs[component.forPopup];
                    if (popup === undefined || popup === null)
                        return;

                    if (Array.isArray(popup)) {
                        popup[0].showed = true;
                        popup[0].$emit('opened', {});
                    }
                    else {
                        popup.showed = true;
                        popup.$emit('opened', {});
                    }
                        
                });
        });
    }
}