<template>
    <div class="vw-pager">
        <div v-if="maxPage > 1" class="vw-pager-paginator">
            <div v-for="page in pagesToShow" :key="page">
                <button v-if="page !== '...'"
                        type="button"
                        class="vm-pager-paginator-button"
                        @click="handleClick(page)">
                    {{ page }}
                </button>
                <span v-else class="vm-pager-paginator-ellipsis">
                    {{ page }}
                </span>
            </div>
        </div>
    </div>
</template>

<script src="./pager.js"></script>
<style src="./pager.scss" lang="scss"></style>