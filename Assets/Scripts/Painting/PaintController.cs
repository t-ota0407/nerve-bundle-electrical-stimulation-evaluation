using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PaintController : MonoBehaviour
{
    [SerializeField]
    private RawImage m_image = null;

    private Texture2D m_texture = null;

    [SerializeField]
    private int m_width = 4;

    [SerializeField]
    private int m_height = 4;

    private Vector2 m_prePos;
    private Vector2 m_TouchPos;

    private float m_clickTime, m_preClickTime;

    public bool isConstraind = false;
    private int constraintX = -1;

    public void OnDrag(BaseEventData arg) //線を描画
    {
        PointerEventData _event = arg as PointerEventData; //タッチの情報取得

        if (_event.button == PointerEventData.InputButton.Left)
        {
            // 押されているときの処理
            m_TouchPos = _event.position; //現在のポインタの座標
            m_clickTime = _event.clickTime; //最後にクリックイベントが送信された時間を取得

            float disTime = m_clickTime - m_preClickTime; //前回のクリックイベントとの時差

            int width = m_width;  //ペンの太さ(ピクセル)
            int height = m_height; //ペンの太さ(ピクセル)

            var dir = m_prePos - m_TouchPos; //直前のタッチ座標との差
            if (disTime > 0.01) dir = new Vector2(0, 0); //0.1秒以上間隔があいたらタッチ座標の差を0にする

            var dist = (int)dir.magnitude; //タッチ座標ベクトルの絶対値

            dir = dir.normalized; //正規化

            //指定のペンの太さ(ピクセル)で、前回のタッチ座標から今回のタッチ座標まで塗りつぶす
            for (int d = 0; d < dist; ++d)
            {
                var p_pos = m_TouchPos + dir * d; //paint position
                p_pos.y -= height / 2.0f;
                p_pos.x -= width / 2.0f;
                for (int h = 0; h < height; ++h)
                {
                    int y = (int)(p_pos.y + h);
                    if (y < 0 || y > m_texture.height) continue; //タッチ座標がテクスチャの外の場合、描画処理を行わない

                    for (int w = 0; w < width; ++w)
                    {
                        int x = (int)(p_pos.x + w);
                        if (x >= 0 && x <= m_texture.width)
                        {
                            m_texture.SetPixel(x, y, new Color(1, 0.23f, 0.03f, 1)); //線を描画
                        }
                    }
                }
            }
            m_texture.Apply();
            m_prePos = m_TouchPos;
            m_preClickTime = m_clickTime;
        }
    }

    public void OnRightClick(BaseEventData arg) //点を描画
    {
        PointerEventData _event = arg as PointerEventData; //タッチの情報取得

        if (_event.button == PointerEventData.InputButton.Right)
        {
            WhitenImage(); // 点を1点しか描画しないために描画前にこれまでの描画を全て消す。

            // 押されているときの処理
            m_TouchPos = _event.position; //現在のポインタの座標

            int width = m_width * 2;  //ペンの太さ(ピクセル)
            int height = m_height * 2; //ペンの太さ(ピクセル)

            var p_pos = m_TouchPos; //paint position
            p_pos.y -= height / 2.0f;
            p_pos.x -= width / 2.0f;

            for (int h = 0; h < height; ++h)
            {
                int y = (int)(p_pos.y + h);
                if (y < 0 || y > m_texture.height) continue; //タッチ座標がテクスチャの外の場合、描画処理を行わない
                for (int w = 0; w < width; ++w)
                {
                    int x = isConstraind ? (int)(constraintX + w) : (int)(p_pos.x + w);
                    if (x >= 0 && x <= m_texture.width)
                    {
                        m_texture.SetPixel(x, y, new Color(0.03f, 0.23f, 1, 1)); //点を描画
                    }
                }
            }

            if (!isConstraind)
            {
                double progress = (p_pos.y - 81) / (569 - 81);
                constraintX = (int)(642 - progress * (642 - 124));
            }

            m_texture.Apply();
        }
    }

    private void Start()
    {
        var rect = m_image.gameObject.GetComponent<RectTransform>().rect;
        m_texture = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGBA32, false);

        WhiteTexture((int)rect.width, (int)rect.height);

        m_image.texture = m_texture;
    }

    private void Update()
    {

    }

    public void WhiteTexture(int width, int height)
    {
        for (int w = 0; w < width; w++)
        {
            for (int h = 0; h < height; h++)
            {
                m_texture.SetPixel(w, h, Color.white);
            }
        }
        m_texture.Apply();
    }

    public void SaveRawImage(string imageFilePath)
    {
        var png = m_texture.EncodeToPNG();
        File.WriteAllBytes(imageFilePath, png);
    }

    public void ReadRawImage(string imageFilePath)
    {
        byte[] fileData;

        if (File.Exists(imageFilePath))
        {
            fileData = ReadPngFile(imageFilePath);
            m_texture.LoadImage(fileData);
            m_texture.Apply();
        }
    }

    private byte[] ReadPngFile(string path)
    {
        FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        BinaryReader bin = new BinaryReader(fileStream);
        byte[] values = bin.ReadBytes((int)bin.BaseStream.Length);

        bin.Close();

        return values;
    }

    public void WhitenImage()
    {
        var rect = m_image.gameObject.GetComponent<RectTransform>().rect;
        int width = (int)rect.width;
        int height = (int)rect.height;

        for (int w = 0; w < width; w++)
        {
            for (int h = 0; h < height; h++)
            {
                m_texture.SetPixel(w, h, Color.white);
            }
        }
        m_texture.Apply();
    }
}
