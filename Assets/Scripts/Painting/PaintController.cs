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

    public void OnDrag(BaseEventData arg) //����`��
    {
        PointerEventData _event = arg as PointerEventData; //�^�b�`�̏��擾

        if (_event.button == PointerEventData.InputButton.Left)
        {
            // ������Ă���Ƃ��̏���
            m_TouchPos = _event.position; //���݂̃|�C���^�̍��W
            m_clickTime = _event.clickTime; //�Ō�ɃN���b�N�C�x���g�����M���ꂽ���Ԃ��擾

            float disTime = m_clickTime - m_preClickTime; //�O��̃N���b�N�C�x���g�Ƃ̎���

            int width = m_width;  //�y���̑���(�s�N�Z��)
            int height = m_height; //�y���̑���(�s�N�Z��)

            var dir = m_prePos - m_TouchPos; //���O�̃^�b�`���W�Ƃ̍�
            if (disTime > 0.01) dir = new Vector2(0, 0); //0.1�b�ȏ�Ԋu����������^�b�`���W�̍���0�ɂ���

            var dist = (int)dir.magnitude; //�^�b�`���W�x�N�g���̐�Βl

            dir = dir.normalized; //���K��

            //�w��̃y���̑���(�s�N�Z��)�ŁA�O��̃^�b�`���W���獡��̃^�b�`���W�܂œh��Ԃ�
            for (int d = 0; d < dist; ++d)
            {
                var p_pos = m_TouchPos + dir * d; //paint position
                p_pos.y -= height / 2.0f;
                p_pos.x -= width / 2.0f;
                for (int h = 0; h < height; ++h)
                {
                    int y = (int)(p_pos.y + h);
                    if (y < 0 || y > m_texture.height) continue; //�^�b�`���W���e�N�X�`���̊O�̏ꍇ�A�`�揈�����s��Ȃ�

                    for (int w = 0; w < width; ++w)
                    {
                        int x = (int)(p_pos.x + w);
                        if (x >= 0 && x <= m_texture.width)
                        {
                            m_texture.SetPixel(x, y, new Color(1, 0.23f, 0.03f, 1)); //����`��
                        }
                    }
                }
            }
            m_texture.Apply();
            m_prePos = m_TouchPos;
            m_preClickTime = m_clickTime;
        }
    }

    public void OnRightClick(BaseEventData arg) //�_��`��
    {
        PointerEventData _event = arg as PointerEventData; //�^�b�`�̏��擾

        if (_event.button == PointerEventData.InputButton.Right)
        {
            WhitenImage(); // �_��1�_�����`�悵�Ȃ����߂ɕ`��O�ɂ���܂ł̕`���S�ď����B

            // ������Ă���Ƃ��̏���
            m_TouchPos = _event.position; //���݂̃|�C���^�̍��W

            int width = m_width * 2;  //�y���̑���(�s�N�Z��)
            int height = m_height * 2; //�y���̑���(�s�N�Z��)

            var p_pos = m_TouchPos; //paint position
            p_pos.y -= height / 2.0f;
            p_pos.x -= width / 2.0f;

            for (int h = 0; h < height; ++h)
            {
                int y = (int)(p_pos.y + h);
                if (y < 0 || y > m_texture.height) continue; //�^�b�`���W���e�N�X�`���̊O�̏ꍇ�A�`�揈�����s��Ȃ�
                for (int w = 0; w < width; ++w)
                {
                    int x = isConstraind ? (int)(constraintX + w) : (int)(p_pos.x + w);
                    if (x >= 0 && x <= m_texture.width)
                    {
                        m_texture.SetPixel(x, y, new Color(0.03f, 0.23f, 1, 1)); //�_��`��
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
