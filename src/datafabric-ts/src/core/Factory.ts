export interface Factory<TEntity, TDto> {
    createEmpty(): TEntity
    createFromDto(data: TDto): TEntity
}