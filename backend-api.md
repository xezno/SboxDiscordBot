# bootleg s&box backend api docs

All endpoints are on the `https://apix.facepunch.com/api/` domain.

## Types

NOTE: Nullable types are suffixed with `?`. This API is subject to change and probably
will do so before release.

### PackageType

An enum specifying the different package types.

Starts at 1:

1. Map
2. Gamemode

### Find Result

|Name           |Type            |Description                                   |
|---------------|----------------|----------------------------------------------|
| type          | `PackageType`  | The package types being returned             |
| assets        | `Package[]`    | A list of matching packages                  |

### Category

|Name           |Type         |Description                                   |
|---------------|-------------|----------------------------------------------|
| title         | `string`    | This category's title                        |
| description   | `string?`   | This category's description                  |
| packages      | `Package[]` | A list of packages within this category      |

### Asset

|Name           |Type       |Description                                   |
|---------------|-----------|----------------------------------------------|
| asset         | `Package` | This asset's package info                    |

### Package

|Name           |Type           |Description                                            |
|---------------|---------------|-------------------------------------------------------|
| org           | `Org`         | The organisation owning this package                  |
| ident         | `string`      | This package's identifier                             |
| title         | `string`      | This package's human-friendly name                    |
| summary       | `string?`     | A short description for this package                  |
| thumb         | `string?`     | A URL to this package's thumbnail                     |
| packageType   | `PackageType` | The package's type                                    |
| updated       | `long`        | UNIX timestamp for when the package was laste updated |
| description   | `string?`     | A longer description for this package                 |
| background    | `string?`     | A URL to this package's in-game background            |
| downloadUrl   | `string?`     | A download URL for this package                       |

### Org

|Name           |Type      |Description                                   |
|---------------|----------|----------------------------------------------|
| ident         | `string` | This organisation's identifier               |
| title         | `string` | This organisation's human-friendly name      |
| description   | `string?`| This organisation's description              |
| thumb         | `string?`| A URL to the organisation's chosen thumbnail |
| socialTwitter | `string?`| A URL to the organisation's Twitter account  |
| socialWeb     | `string?`| A URL to the organisation's website          |

## Endpoints

### `/sbox/menu/index`

#### Params

None

#### Returns

A list of `Category` objects.

---

### `/sbox/asset/get`

#### Params

`id`: An identifier (`org`.`package`) for the asset you wish to retrive info about.

#### Returns

An `Asset`

---

### `/sbox/asset/find`

#### Params

`type`: `map` or `gamemode`.

#### Returns

A `FindResult` object.

## Other

### Finding an Org

There's no known endpoint for organisations. This is the closest you can get:

1. Visit several endpoints (/asset/find?type=map|gamemode, /menu/index) and collect a list of orgs with their gamemode idents
2. Search that list for an org that matches the ident we're looking for
3. Visit /asset/get?id=(org).(ident)
4. Get the title & description for the org from this
