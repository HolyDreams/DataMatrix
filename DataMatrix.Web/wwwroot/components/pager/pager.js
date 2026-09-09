export default {
    props: {
        currentPage: {
            type: Number,
            default: 1
        },
        siblingPagesCount: {
            type: Number,
            default: 1
        },
        maxPage: {
            type: Number,
            required: true
        }
    },
    computed: {
        pagesToShow() {
            const total = this.maxPage;
            const current = this.currentPage;
            const delta = this.siblingPagesCount;

            const start = Math.max(1, current - delta);
            const end = Math.min(total, current + delta);

            const range = [];
            if (start > 1) {
                range.push(1);
                if (start > 2) {
                    range.push('...');
                }
            }

            for (let i = start; i <= end; i++) {
                range.push(i);
            }

            if (end < total) {
                if (total - end > 1) {
                    range.push('...');
                }
                range.push(total);
            }
            return range;
        }
    },
    methods: {
        handleClick(page) {
            this.$emit('page-change', page);
        }
    }
}