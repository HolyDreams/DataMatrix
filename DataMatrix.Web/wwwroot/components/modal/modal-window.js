export default {
    data: function() {
        return {
            showed: false
        };
    },
    props: {
        name: {
            type: String
        },
        header: {
            type: String,
            default: ''
        },
        animationEnabled: {
            type: Boolean,
            default: true
        },
        canClose: {
            type: Boolean,
            default: true
        },
        topIcon: {
            type: String,
            default: ''
        }
    }
}