import { Injectable } from '@angular/core';
import { HttpInternalService } from './http-internal.service';
import { Post } from '../models/post/post';
import { NewReaction } from '../models/reactions/newReaction';
import { NewPost } from '../models/post/new-post';

@Injectable({ providedIn: 'root' })
export class PostService {
    public routePrefix = '/api/posts';

    constructor(private httpService: HttpInternalService) {}

    public getPosts() {
        return this.httpService.getFullRequest<Post[]>(`${this.routePrefix}`);
    }

    public createPost(post: NewPost) {
        return this.httpService.postFullRequest<Post>(`${this.routePrefix}`, post);
    }

    public likePost(reaction: NewReaction) {
        return this.httpService.postFullRequest<Post>(`${this.routePrefix}/like`, reaction);
    }
    public deletePost(post: Post) {
        return this.httpService.deleteRequest<Post>(`${this.routePrefix}/${post.id}`);
    }

    public updatePost(post: Post) {
        return this.httpService.putFullRequest<Post>(`${this.routePrefix}`, post);
    }

    public sharePost(post: Post, email) {
        return this.httpService.postRequest<Post>(`${this.routePrefix}/${email}`, post);
    }
}
