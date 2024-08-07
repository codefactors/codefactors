// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

export class DataFabricConfiguration {
    routes = ['^/employers/[0-9a-fA-F-]{36}/employees/[0-9a-fA-F-]{36}$', 'Employee']
}