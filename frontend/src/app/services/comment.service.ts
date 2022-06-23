import { Injectable } from '@angular/core';
import { HttpInternalService } from './http-internal.service';
import { NewComment } from '../models/comment/new-comment';
import { Comment } from '../models/comment/comment';
import { NewReaction } from '../models/reactions/newReaction';
import {Post} from '../models/post/post';

@Injectable({ providedIn: 'root' })
export class CommentService {
    public routePrefix = '/api/comments';

    constructor(private httpService: HttpInternalService) {}

    public createComment(post: NewComment) {
        return this.httpService.postFullRequest<Comment>(`${this.routePrefix}`, post);
    }

    public likeComment(reaction: NewReaction) {
        return this.httpService.postFullRequest<Comment>(`${this.routePrefix}/like`, reaction);
    }

    public deleteComment(comment: Comment) {
        return this.httpService.deleteRequest<Comment>(`${this.routePrefix}/${comment.id}`);
    }
    public updateComment(comment: Comment) {
        return this.httpService.putFullRequest<Comment>(`${this.routePrefix}`, comment);
    }
}
