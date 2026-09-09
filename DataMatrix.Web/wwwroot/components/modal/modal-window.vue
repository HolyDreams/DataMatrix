<template>
    <div class="vw-modal-window-wrapper" v-if="showed">
        <div class="vw vw-modal-window" :class="{ 'animated': animationEnabled }" :id="name">
            <div class="vw-modal-window-header space-between-flex-container">
                <div class="flex-vertical">
                    <div v-if="!!topIcon" class="vw-modal-window-icon-top flex">
                        <img :src="topIcon" />
                    </div>
                    <div class="vw-modal-window-header-label">
                        {{ header }}
                    </div>
                </div>
                <div v-if=canClose class="vw-modal-window-header-close-icon-container">
                    <div class="vw-modal-window-header-close-icon" @click="function() { showed = false; $emit('closed'); }">
                        <img src="/icons/common/close.svg" />
                    </div>
                </div>
            </div>
            <div class="vw-modal-window-content-wrapper">
                <slot></slot>
            </div>
        </div>
    </div>
</template>
<script src="./modal-window.js"></script>
<style src="./modal-window.scss" lang="scss"></style>