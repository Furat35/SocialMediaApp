<template lang="pug">
    .ig-stories-bar
        div.story-circle.ig-story(v-for="(story, index) in stories" :key="index")
            <img :src="story.image" alt="story" />
</template>

<script lang="ts">

export default {
    data() {
        return {
            stories: [
                { image: 'https://randomuser.me/api/portraits/women/1.jpg' },
                { image: 'https://randomuser.me/api/portraits/men/2.jpg' },
                { image: 'https://randomuser.me/api/portraits/women/3.jpg' },
                { image: 'https://randomuser.me/api/portraits/men/4.jpg' },
                { image: 'https://randomuser.me/api/portraits/women/5.jpg' }
            ],
        }
    }
}
</script>