export default {
    props: {
        forModal: {
            type: String,
            required: true
        }
    },

    mounted: function() {
        var component = this;
        Array.from(component.$el.children).forEach(function(child) {
            child.addEventListener('click',
                function() {
                    var modal = component.$root.$refs[component.forModal];
                    if (Array.isArray(modal) && modal.length >= 1)
                        modal = modal[0];
                    if (modal === undefined || modal === null)
                        return;

                    modal.showed = true;
                });
        });
    }
}