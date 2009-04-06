﻿Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.IO
Imports System.Windows.Forms
Imports Microsoft.DirectX
Imports Microsoft.DirectX.Direct3D

Namespace wow2collada

    Public Structure MCNK
        Dim flags As UInt32
        Dim IndexX As UInt32
        Dim IndexY As UInt32
        Dim nLayers As UInt32
        Dim nDoodadRefs As UInt32
        Dim offsHeight As UInt32
        Dim offsNormal As UInt32
        Dim offsLayer As UInt32
        Dim offsRefs As UInt32
        Dim offsAlpha As UInt32
        Dim sizeAlpha As UInt32
        Dim offsShadow As UInt32
        Dim sizeShadow As UInt32
        Dim areaid As UInt32
        Dim nMapObjRefs As UInt32
        Dim holes As UInt32
        Dim predTex As UInt32
        Dim noEffectDoodad As UInt32
        Dim offsSndEmitters As UInt32
        Dim nSndEmitters As UInt32
        Dim offsLiquid As UInt32
        Dim sizeLiquid As UInt32
        Dim Position As Vector3
        Dim offsColorValues As UInt32
        Dim props As UInt32
        Dim effectId As UInt32
        Dim HeightMap As Single()
        Dim HeightMap9x9 As Single(,)
        Dim HeightMap8x8 As Single(,)
    End Structure

    Public Structure M2Placement
        Public MMDX_ID As UInt32
        Public ID As UInt32
        Public Position As Vector3
        Public Orientation As Vector3
        Public Scale As Single
    End Structure

    Public Structure WMOPlacement
        Public MWMO_ID As UInt32
        Public ID As UInt32
        Public Position As Vector3
        Public Orientation As Vector3
        Public UpperExtents As Vector3
        Public LowerExtents As Vector3
        Public DoodadSetIndex As UInt16
        Public NameSetIndex As UInt32
    End Structure

    Public Structure DoodadSet
        Public name As String
        Public index As UInt32
        Public count As UInt32
    End Structure

    Public Structure Doodad
        Public ModelFile As String
        Public Position As Vector3
        Public Orientation As Quaternion
        Public Scale As Single
        Public LightingColor As Color
    End Structure

    Public Structure BoneIndices
        Public BoneIndex1 As Byte
        Public BoneIndex2 As Byte
        Public BoneIndex3 As Byte
        Public BoneIndex4 As Byte
    End Structure

    Public Structure Triangle
        Public VertexIndex1 As UInt16
        Public VertexIndex2 As UInt16
        Public VertexIndex3 As UInt16
    End Structure

    Public Structure SubMesh
        Public ID As UInt32
        Public StartVertex As UInt16
        Public nVertices As UInt16
        Public StartTriangle As UInt16
        Public nTriangles As UInt16
        Public StartBones As UInt16
        Public nBones As UInt16
        Public RootBone As UInt16
        Public Position As Vector3
    End Structure

    Public Structure TextureUnit
        Dim IsStatic As Boolean
        Dim RenderOrder As UInt16
        Dim SubmeshIndex1 As UInt16
        Dim SubmeshIndex2 As UInt16
        Dim ColorIndex As UInt16
        Dim RenderFlags As UInt16
        Dim TexUnitNumber As UInt16
        Dim Texture As UInt16
        Dim TexUnitNumber2 As UInt16
        Dim Transparency As UInt16
        Dim TextureAnim As UInt16
    End Structure

    Public Structure Vertex
        Public Position As Vector3
        Public BoneWeights As Byte()
        Public BoneIndices As Byte()
        Public Normal As Vector3
        Public TextureCoords As Vector2
    End Structure

    Public Structure M2Texture
        Public Type As UInt32
        Public Flags As UInt16
        Public lenFilename As UInt32
        Public ofsFilename As UInt32
        Public Filename As String
    End Structure

    Class HelperFunctions

        Public Function StrRev(ByVal value As String) As String
            If value.Length > 1 Then
                Dim workingValue As New System.Text.StringBuilder
                For position As Int32 = value.Length - 1 To 0 Step -1
                    workingValue.Append(value.Chars(position))
                Next
                Return workingValue.ToString
            Else
                Return value
            End If
        End Function

        Public Function GetZeroDelimitedString(ByVal Stack() As Byte, ByVal Pos As UInt32) As String
            Dim out As String = ""

            While Stack(Pos) <> 0
                out &= Chr(Stack(Pos))
                Pos += 1
            End While

            Return out
        End Function

        Public Function GetAllZeroDelimitedStrings(ByVal Stack() As Byte) As String()
            Dim d As String = Chr(0)
            Return Encoding.ASCII.GetString(Stack).Split(d, options:=System.StringSplitOptions.RemoveEmptyEntries)
        End Function

        Public Function GetHeightMap9x9FromHeightMap(ByVal HeightMap() As Single)
            Dim Out(8, 8)

            '  1    2    3    4    5    6    7    8    9       Row 0
            '    10   11   12   13   14   15   16   17         Row 1
            '  18   19   20   21   22   23   24   25   26      Row 2
            '    27   28   29   30   31   32   33   34         Row 3
            '  35   36   37   38   39   40   41   42   43      Row 4
            '    44   45   46   47   48   49   50   51         Row 5
            '  52   53   54   55   56   57   58   59   60      Row 6
            '    61   62   63   64   65   66   67   68         Row 7
            '  69   70   71   72   73   74   75   76   77      Row 8
            '    78   79   80   81   82   83   84   85         Row 9
            '  86   87   88   89   90   91   92   93   94      Row 10
            '    95   96   97   98   99   100  101  102        Row 11
            ' 103  104  105  106  107  108  109  110  111      Row 12
            '   112  113  114  115  116  117  118  119         Row 13
            ' 120  121  122  123  124  125  126  127  128      Row 14
            '   129  130  131  132  133  134  135  136         Row 15
            ' 137  138  139  140  141  142  143  144  145      Row 16

            For r As Integer = 0 To 8
                For c As Integer = 0 To 8
                    Out(r, c) = HeightMap(r * 17 + c)
                Next
            Next

            Return Out
        End Function

        Public Function GetHeightMap8x8FromHeightMap(ByVal HeightMap() As Single)
            Dim Out(7, 7)

            '  1    2    3    4    5    6    7    8    9       Row 0
            '    10   11   12   13   14   15   16   17         Row 1
            '  18   19   20   21   22   23   24   25   26      Row 2
            '    27   28   29   30   31   32   33   34         Row 3
            '  35   36   37   38   39   40   41   42   43      Row 4
            '    44   45   46   47   48   49   50   51         Row 5
            '  52   53   54   55   56   57   58   59   60      Row 6
            '    61   62   63   64   65   66   67   68         Row 7
            '  69   70   71   72   73   74   75   76   77      Row 8
            '    78   79   80   81   82   83   84   85         Row 9
            '  86   87   88   89   90   91   92   93   94      Row 10
            '    95   96   97   98   99   100  101  102        Row 11
            ' 103  104  105  106  107  108  109  110  111      Row 12
            '   112  113  114  115  116  117  118  119         Row 13
            ' 120  121  122  123  124  125  126  127  128      Row 14
            '   129  130  131  132  133  134  135  136         Row 15
            ' 137  138  139  140  141  142  143  144  145      Row 16

            For r As Integer = 0 To 7
                For c As Integer = 0 To 7
                    Out(r, c) = HeightMap(9 + r * 17 + c)
                Next
            Next

            Return Out
        End Function

        Public Sub CreateVertexBuffer(ByRef frm As RenderForm, ByVal _MD20 As wow2collada.FileReaders.M2, ByVal _SKIN As wow2collada.FileReaders.SKIN)

            frm.NUM_TRIANGLES = _SKIN.Triangles.Length
            frm.NUM_POINTS = _SKIN.Triangles.Length * 3


            frm.m_VertexBuffer = New VertexBuffer(GetType(CustomVertex.PositionNormalColored), frm.NUM_POINTS, frm.m_Device, 0, CustomVertex.PositionNormalColored.Format, Pool.Default)

            ' Lock the vertex buffer. 
            ' Lock returns an array of PositionNormalColored objects.
            Dim vertices As CustomVertex.PositionNormalColored() = CType(frm.m_VertexBuffer.Lock(0, 0), CustomVertex.PositionNormalColored())


            For i As Integer = 0 To _SKIN.Triangles.Length - 1
                vertices(i * 3 + 0) = New CustomVertex.PositionNormalColored(_MD20.Vertices(_SKIN.Triangles(i).VertexIndex1).Position.X, _
                                                                             _MD20.Vertices(_SKIN.Triangles(i).VertexIndex1).Position.Z, _
                                                                             -_MD20.Vertices(_SKIN.Triangles(i).VertexIndex1).Position.Y, _
                                                                             _MD20.Vertices(_SKIN.Triangles(i).VertexIndex1).Normal.X, _
                                                                             _MD20.Vertices(_SKIN.Triangles(i).VertexIndex1).Normal.Z, _
                                                                             -_MD20.Vertices(_SKIN.Triangles(i).VertexIndex1).Normal.Y, _
                                                                             Color.Green.ToArgb)
                vertices(i * 3 + 1) = New CustomVertex.PositionNormalColored(_MD20.Vertices(_SKIN.Triangles(i).VertexIndex2).Position.X, _
                                                                             _MD20.Vertices(_SKIN.Triangles(i).VertexIndex2).Position.Z, _
                                                                             -_MD20.Vertices(_SKIN.Triangles(i).VertexIndex2).Position.Y, _
                                                                             _MD20.Vertices(_SKIN.Triangles(i).VertexIndex2).Normal.X, _
                                                                             _MD20.Vertices(_SKIN.Triangles(i).VertexIndex2).Normal.Z, _
                                                                             -_MD20.Vertices(_SKIN.Triangles(i).VertexIndex2).Normal.Y, _
                                                                             Color.Green.ToArgb)
                vertices(i * 3 + 2) = New CustomVertex.PositionNormalColored(_MD20.Vertices(_SKIN.Triangles(i).VertexIndex3).Position.X, _
                                                                             _MD20.Vertices(_SKIN.Triangles(i).VertexIndex3).Position.Z, _
                                                                             -_MD20.Vertices(_SKIN.Triangles(i).VertexIndex3).Position.Y, _
                                                                             _MD20.Vertices(_SKIN.Triangles(i).VertexIndex3).Normal.X, _
                                                                             _MD20.Vertices(_SKIN.Triangles(i).VertexIndex3).Normal.Z, _
                                                                             -_MD20.Vertices(_SKIN.Triangles(i).VertexIndex3).Normal.Y, _
                                                                             Color.Green.ToArgb)

            Next

            frm.m_VertexBuffer.Unlock()
        End Sub

        Public Sub CreateVertexBufferTextured(ByRef frm As RenderForm, ByVal _MD20 As wow2collada.FileReaders.M2, ByVal _SKIN As wow2collada.FileReaders.SKIN)

            'if the texture doesn't exist, then don't use it...
            Dim TextureFileName As String = "d:\temp\mpq\" & _MD20.Textures(_SKIN.TextureUnits(0).Texture).Filename
            If Not File.Exists(TextureFileName) Then
                frm.m_Texture = Nothing
                CreateVertexBuffer(frm, _MD20, _SKIN)
            Else


                frm.NUM_TRIANGLES = _SKIN.Triangles.Length
                frm.NUM_POINTS = _SKIN.Triangles.Length * 3


                frm.m_VertexBuffer = New VertexBuffer(GetType(CustomVertex.PositionNormalTextured), frm.NUM_POINTS, frm.m_Device, 0, CustomVertex.PositionNormalTextured.Format, Pool.Default)

                ' Load the Texture (for now only the first texture and with no checks)
                Dim ms As New System.IO.MemoryStream
                ms = LoadBLP(TextureFileName)
                frm.m_Texture = TextureLoader.FromStream(frm.m_Device, ms)
                'frm.PictureBox1.Image = LoadBLP(frm, "d:\temp\mpq\" & _MD20.Textures(_SKIN.TextureUnits(0).Texture).Filename)

                ' Lock the vertex buffer. 
                ' Lock returns an array of PositionNormalColored objects.
                Dim vertices As CustomVertex.PositionNormalTextured() = CType(frm.m_VertexBuffer.Lock(0, 0), CustomVertex.PositionNormalTextured())


                For i As Integer = 0 To _SKIN.Triangles.Length - 1
                    vertices(i * 3 + 0) = New CustomVertex.PositionNormalTextured(_MD20.Vertices(_SKIN.Triangles(i).VertexIndex1).Position.X, _
                                                                                 _MD20.Vertices(_SKIN.Triangles(i).VertexIndex1).Position.Z, _
                                                                                 -_MD20.Vertices(_SKIN.Triangles(i).VertexIndex1).Position.Y, _
                                                                                 _MD20.Vertices(_SKIN.Triangles(i).VertexIndex1).Normal.X, _
                                                                                 _MD20.Vertices(_SKIN.Triangles(i).VertexIndex1).Normal.Z, _
                                                                                 -_MD20.Vertices(_SKIN.Triangles(i).VertexIndex1).Normal.Y, _
                                                                                 _MD20.Vertices(_SKIN.Triangles(i).VertexIndex1).TextureCoords.X, _
                                                                                 _MD20.Vertices(_SKIN.Triangles(i).VertexIndex1).TextureCoords.Y)
                    vertices(i * 3 + 1) = New CustomVertex.PositionNormalTextured(_MD20.Vertices(_SKIN.Triangles(i).VertexIndex2).Position.X, _
                                                                                 _MD20.Vertices(_SKIN.Triangles(i).VertexIndex2).Position.Z, _
                                                                                 -_MD20.Vertices(_SKIN.Triangles(i).VertexIndex2).Position.Y, _
                                                                                 _MD20.Vertices(_SKIN.Triangles(i).VertexIndex2).Normal.X, _
                                                                                 _MD20.Vertices(_SKIN.Triangles(i).VertexIndex2).Normal.Z, _
                                                                                 -_MD20.Vertices(_SKIN.Triangles(i).VertexIndex2).Normal.Y, _
                                                                                 _MD20.Vertices(_SKIN.Triangles(i).VertexIndex2).TextureCoords.X, _
                                                                                 _MD20.Vertices(_SKIN.Triangles(i).VertexIndex2).TextureCoords.Y)
                    vertices(i * 3 + 2) = New CustomVertex.PositionNormalTextured(_MD20.Vertices(_SKIN.Triangles(i).VertexIndex3).Position.X, _
                                                                                 _MD20.Vertices(_SKIN.Triangles(i).VertexIndex3).Position.Z, _
                                                                                 -_MD20.Vertices(_SKIN.Triangles(i).VertexIndex3).Position.Y, _
                                                                                 _MD20.Vertices(_SKIN.Triangles(i).VertexIndex3).Normal.X, _
                                                                                 _MD20.Vertices(_SKIN.Triangles(i).VertexIndex3).Normal.Z, _
                                                                                 -_MD20.Vertices(_SKIN.Triangles(i).VertexIndex3).Normal.Y, _
                                                                                 _MD20.Vertices(_SKIN.Triangles(i).VertexIndex3).TextureCoords.X, _
                                                                                 _MD20.Vertices(_SKIN.Triangles(i).VertexIndex3).TextureCoords.Y)

                Next

                frm.m_VertexBuffer.Unlock()
            End If
        End Sub

        Public Sub CreateVertexBufferFromADT(ByRef frm As RenderForm, ByVal ADT As wow2collada.FileReaders.ADT)
            'do the 9x9 matrix for now and only of the last mcnk
            '  0/0  0/1  0/2  0/3  0/4  0/5  0/6  0/7  0/8
            '  1/0  1/1  1/2  1/3  1/4  1/5  1/6  1/7  1/8
            '  2/0  2/1  2/2  2/3  2/4  2/5  2/6  2/7  2/8
            '  3/0  3/1  3/2  3/3  3/4  3/5  3/6  3/7  3/8
            '  4/0  4/1  4/2  4/3  4/4  4/5  4/6  4/7  4/8
            '  5/0  5/1  5/2  5/3  5/4  5/5  5/6  5/7  5/8
            '  6/0  6/1  6/2  6/3  6/4  6/5  6/6  6/7  6/8
            '  7/0  7/1  7/2  7/3  7/4  7/5  7/6  7/7  7/8
            '  8/0  8/1  8/2  8/3  8/4  8/5  8/6  8/7  8/8

            With ADT.MCNKs(ADT.MCNKs.Length - 1)


                frm.NUM_TRIANGLES = 8 * 8 * 2
                frm.NUM_POINTS = frm.NUM_TRIANGLES * 3

                frm.m_VertexBuffer = New VertexBuffer(GetType(CustomVertex.PositionNormalColored), frm.NUM_POINTS, frm.m_Device, 0, CustomVertex.PositionNormalColored.Format, Pool.Default)

                ' Lock the vertex buffer. 
                ' Lock returns an array of PositionNormalColored objects.
                Dim vertices As CustomVertex.PositionNormalColored() = CType(frm.m_VertexBuffer.Lock(0, 0), CustomVertex.PositionNormalColored())


                For x As Integer = 0 To 7
                    For y As Integer = 0 To 7
                        vertices((x + 8 * y) * 6 + 0) = New CustomVertex.PositionNormalColored(x + 0, y + 0, .HeightMap9x9(x + 0, y + 0), 0, 0, 1, Color.Green.ToArgb)
                        vertices((x + 8 * y) * 6 + 1) = New CustomVertex.PositionNormalColored(x + 0, y + 1, .HeightMap9x9(x + 0, y + 1), 0, 0, 1, Color.Green.ToArgb)
                        vertices((x + 8 * y) * 6 + 2) = New CustomVertex.PositionNormalColored(x + 1, y + 0, .HeightMap9x9(x + 1, y + 0), 0, 0, 1, Color.Green.ToArgb)
                        vertices((x + 8 * y) * 6 + 3) = New CustomVertex.PositionNormalColored(x + 0, y + 1, .HeightMap9x9(x + 0, y + 1), 0, 0, 1, Color.Green.ToArgb)
                        vertices((x + 8 * y) * 6 + 4) = New CustomVertex.PositionNormalColored(x + 1, y + 0, .HeightMap9x9(x + 1, y + 0), 0, 0, 1, Color.Green.ToArgb)
                        vertices((x + 8 * y) * 6 + 5) = New CustomVertex.PositionNormalColored(x + 1, y + 1, .HeightMap9x9(x + 1, y + 1), 0, 0, 1, Color.Green.ToArgb)
                    Next
                Next

            End With
            frm.m_VertexBuffer.Unlock()
        End Sub

        Public Function LoadBLP(ByVal FileName As String) As Stream
            Dim br As New BinaryReader(File.OpenRead(FileName))

            If br.ReadChars(4) <> "BLP2" Then Return Nothing

            Dim bType As UInt32 = br.ReadUInt32
            Dim bEnc As Byte = br.ReadByte
            Dim bAlphaDepth As Byte = br.ReadByte
            Dim bAlphaEnc As Byte = br.ReadByte
            Dim bHasMips As Byte = br.ReadByte
            Dim bWidth As UInt32 = br.ReadUInt32
            Dim bHeight As UInt32 = br.ReadUInt32
            Dim bOffsets As UInt32() = New UInt32(15) {br.ReadUInt32, br.ReadUInt32, br.ReadUInt32, br.ReadUInt32, _
                                                       br.ReadUInt32, br.ReadUInt32, br.ReadUInt32, br.ReadUInt32, _
                                                       br.ReadUInt32, br.ReadUInt32, br.ReadUInt32, br.ReadUInt32, _
                                                       br.ReadUInt32, br.ReadUInt32, br.ReadUInt32, br.ReadUInt32}
            Dim bLengths As UInt32() = New UInt32(15) {br.ReadUInt32, br.ReadUInt32, br.ReadUInt32, br.ReadUInt32, _
                                                       br.ReadUInt32, br.ReadUInt32, br.ReadUInt32, br.ReadUInt32, _
                                                       br.ReadUInt32, br.ReadUInt32, br.ReadUInt32, br.ReadUInt32, _
                                                       br.ReadUInt32, br.ReadUInt32, br.ReadUInt32, br.ReadUInt32}
            Dim Palette(255) As Color
            For i As Integer = 0 To 255
                Palette(i) = Color.FromArgb(br.ReadByte, br.ReadByte, br.ReadByte, br.ReadByte)
            Next

            br.BaseStream.Position = bOffsets(0) 'we only care about the first MIP and assume it is the most relevant one...

            Dim BitmapFromBLP As Bitmap = New Bitmap(bWidth, bHeight, Imaging.PixelFormat.Format32bppArgb)

            If (bType = 1 And bEnc = 2 And bAlphaDepth = 1) Then ' DXT1
                For y As Integer = 0 To bWidth - 1 Step 4
                    For x As Integer = 0 To bHeight - 1 Step 4
                        Dim _RGBValues As UInt16() = New UInt16(1) {br.ReadUInt16, br.ReadUInt16}
                        Dim _ColorLUT As UInt32 = br.ReadUInt32

                        Dim ColorLUT As Byte(,) = New Byte(3, 3) {{(_ColorLUT >> 30 And 3), (_ColorLUT >> 28 And 3), (_ColorLUT >> 26 And 3), (_ColorLUT >> 24 And 3)}, _
                                                                  {(_ColorLUT >> 22 And 3), (_ColorLUT >> 20 And 3), (_ColorLUT >> 18 And 3), (_ColorLUT >> 16 And 3)}, _
                                                                  {(_ColorLUT >> 14 And 3), (_ColorLUT >> 12 And 3), (_ColorLUT >> 10 And 3), (_ColorLUT >> 8 And 3)}, _
                                                                  {(_ColorLUT >> 6 And 3), (_ColorLUT >> 4 And 3), (_ColorLUT >> 2 And 3), (_ColorLUT And 3)}}
                        Dim RGBValues As Color() = New Color(3) {}
                        RGBValues(0) = Color.FromArgb(255, _RGBValues(0) >> 8 And 255, _RGBValues(0) >> 3 And 255, _RGBValues(0) << 3 And 255)
                        RGBValues(1) = Color.FromArgb(255, _RGBValues(1) >> 8 And 255, _RGBValues(1) >> 3 And 255, _RGBValues(1) << 3 And 255)

                        If RGBValues(0).ToArgb > RGBValues(1).ToArgb Then
                            RGBValues(2) = Color.FromArgb(255, 2 / 3 * RGBValues(0).R + 1 / 3 * RGBValues(1).R, 2 / 3 * RGBValues(0).G + 1 / 3 * RGBValues(1).G, 2 / 3 * RGBValues(0).B + 1 / 3 * RGBValues(1).B)
                            RGBValues(3) = Color.FromArgb(255, 1 / 3 * RGBValues(0).R + 2 / 3 * RGBValues(1).R, 1 / 3 * RGBValues(0).G + 2 / 3 * RGBValues(1).G, 1 / 3 * RGBValues(0).B + 2 / 3 * RGBValues(1).B)
                        Else
                            RGBValues(2) = Color.FromArgb(255, 1 / 2 * RGBValues(0).R + 1 / 2 * RGBValues(1).R, 1 / 2 * RGBValues(0).G + 1 / 2 * RGBValues(1).G, 1 / 2 * RGBValues(0).B + 1 / 2 * RGBValues(1).B)
                            RGBValues(3) = Color.FromArgb(0, 0, 0, 0)
                        End If

                        For y1 As Integer = 0 To 3
                            For x1 As Integer = 0 To 3
                                Dim ci As Integer = ColorLUT(3 - y1, 3 - x1)
                                BitmapFromBLP.SetPixel(x + x1, y + y1, Color.FromArgb(RGBValues(ci).A, RGBValues(ci).R, RGBValues(ci).G, RGBValues(ci).B))
                            Next
                        Next

                    Next
                Next

            ElseIf (bType = 1 And bEnc = 2 And bAlphaDepth = 8 And bAlphaEnc = 1) Then ' DXT3
                For y As Integer = 0 To bWidth Step 4
                    For x As Integer = 0 To bHeight Step 4
                        Dim _AlphaMap As Byte() = New Byte(7) {br.ReadByte, br.ReadByte, br.ReadByte, br.ReadByte, br.ReadByte, br.ReadByte, br.ReadByte, br.ReadByte}
                        Dim _RGBValues As UInt16() = New UInt16(1) {br.ReadUInt16, br.ReadUInt16}
                        Dim _ColorLUT As UInt32 = br.ReadUInt32

                        Dim ColorLUT As Byte(,) = New Byte(3, 3) {{(_ColorLUT >> 30 And 3), (_ColorLUT >> 28 And 3), (_ColorLUT >> 26 And 3), (_ColorLUT >> 24 And 3)}, _
                                                                  {(_ColorLUT >> 22 And 3), (_ColorLUT >> 20 And 3), (_ColorLUT >> 18 And 3), (_ColorLUT >> 16 And 3)}, _
                                                                  {(_ColorLUT >> 14 And 3), (_ColorLUT >> 12 And 3), (_ColorLUT >> 10 And 3), (_ColorLUT >> 8 And 3)}, _
                                                                  {(_ColorLUT >> 6 And 3), (_ColorLUT >> 4 And 3), (_ColorLUT >> 2 And 3), (_ColorLUT And 3)}}
                        'alpha calculation probably wrong...
                        Dim AlphaMap As Byte(,) = New Byte(3, 3) {{_AlphaMap(0) >> 4 And 15, _AlphaMap(0) And 15, _AlphaMap(1) >> 4 And 15, _AlphaMap(1) And 15}, _
                                                                  {_AlphaMap(2) >> 4 And 15, _AlphaMap(2) And 15, _AlphaMap(3) >> 4 And 15, _AlphaMap(3) And 15}, _
                                                                  {_AlphaMap(4) >> 4 And 15, _AlphaMap(4) And 15, _AlphaMap(5) >> 4 And 15, _AlphaMap(5) And 15}, _
                                                                  {_AlphaMap(6) >> 4 And 15, _AlphaMap(6) And 15, _AlphaMap(7) >> 4 And 15, _AlphaMap(7) And 15}}

                        Dim RGBValues As Color() = New Color(3) {}
                        RGBValues(0) = Color.FromArgb(0, _RGBValues(0) >> 8 And 255, _RGBValues(0) >> 3 And 255, _RGBValues(0) << 3 And 255)
                        RGBValues(1) = Color.FromArgb(0, _RGBValues(1) >> 8 And 255, _RGBValues(1) >> 3 And 255, _RGBValues(1) << 3 And 255)

                        If _RGBValues(0) > _RGBValues(1) Then
                            RGBValues(2) = Color.FromArgb(0, 2 / 3 * RGBValues(0).R + 1 / 3 * RGBValues(1).R, 2 / 3 * RGBValues(0).G + 1 / 3 * RGBValues(1).G, 2 / 3 * RGBValues(0).B + 1 / 3 * RGBValues(1).B)
                            RGBValues(3) = Color.FromArgb(0, 1 / 3 * RGBValues(0).R + 2 / 3 * RGBValues(1).R, 1 / 3 * RGBValues(0).G + 2 / 3 * RGBValues(1).G, 1 / 3 * RGBValues(0).B + 2 / 3 * RGBValues(1).B)
                        Else
                            Debug.Print("Weird Values found in file...")
                        End If

                        For y1 As Integer = 0 To 3
                            For x1 As Integer = 0 To 3
                                Dim ci As Integer = ColorLUT(3 - y1, 3 - x1)
                                Dim al As Integer = AlphaMap(3 - y1, 3 - x1)
                                BitmapFromBLP.SetPixel(x + x1, y + y1, Color.FromArgb(al, RGBValues(ci).R, RGBValues(ci).G, RGBValues(ci).B))
                            Next
                        Next

                    Next
                Next

            Else
                Return Nothing 'we don't support this yet...
            End If

            Dim iStream As New System.IO.MemoryStream
            BitmapFromBLP.Save(iStream, Imaging.ImageFormat.Png)
            iStream.Position = 0
            Return iStream

        End Function

    End Class

End Namespace
